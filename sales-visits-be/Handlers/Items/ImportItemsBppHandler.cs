using ClosedXML.Excel;
using entities;
using entities.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Enums;
using sales_visits_be.Models.Items;
using sales_visits_be.Service;

namespace sales_visits_be.Handlers.Items;

public class ImportItemsBppHandler : IRequestHandler<ImportItemsBppRequest, ItemsResponse>
{
    private readonly ApplicationDbContext _db;
    private readonly BlobService _service;

    public ImportItemsBppHandler(BlobService service, ApplicationDbContext db)
    {
        _service = service;
        _db = db;
    }

    public async Task<ItemsResponse> Handle(ImportItemsBppRequest request, CancellationToken cancellationToken)
    {
        var typeDictionary = new Dictionary<string, string>
        {
            {"CONV", "CONVENTIONAL"},
            {"HYB", "HYBRID"},
            {"MF", "MF"},
            {"AIR ACCU", "AIR ACCU"}
        };
        var rowsInserted = 0;
        var rowsUpdated = 0;
        foreach (var blobName in request.BlobNames)
        {
            var (stream, contentType) = await _service.GetObjectAsync(blobName);
            if (stream == null)
            {
                return new ItemsResponse
                {
                    IsSuccess = false,
                    Message = "No file found"
                };
            }

            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet(1);
            var usedRows = ws.RangeUsed().RangeAddress;

            var headerRow = GetStartingIndexHelper(ws);
            var headers = new Dictionary<string, int>();
            var lastCol = usedRows.LastAddress.ColumnNumber;
            for (int col = 1; col <= lastCol; col++)
            {
                var value = ws.Cell(headerRow, col).GetString().Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    headers.TryAdd(value, col);
                }
            }

            var lastRow = usedRows.LastAddress.RowNumber;
            var r = headerRow + 1;

            var importedItems = new List<ItemDTO>();
            try
            {
                while (r <= lastRow)
                {
                    var cell = ws.Cell(r, 1);
                    if (!cell.IsEmpty())
                    {
                        var row = ws.Row(r);
                        var rowData = row.CellsUsed().Select(c => c.GetString()).ToList();
                        if(cell.ToString() == "Kode Item" || rowData.Count != 6)
                        {
                            r++;
                        }
                        var brandAndType = ws.Cell(r, headers["Jenis"]).GetString().Trim();
                        var brand = "";
                        var type = "";
                        if(brandAndType != "AIR ACCU")
                        {
                            brand = brandAndType.Split(' ')[0];
                            type = brandAndType.Split(' ')[1];
                        }
                        else
                        {
                            brand = brandAndType;
                            type = brandAndType;
                        }
                        
                    
                        importedItems.Add(new ItemDTO
                        {
                            Code = ws.Cell(r, headers["Kode Item"]).GetString(),
                            Name = ws.Cell(r, headers["Nama Item"]).GetString(),
                            Brand = brand,
                            Type = typeDictionary[type],
                            Quantity = ws.Cell(r, headers["Stok"]).GetValue<int>(),
                            Unit = ws.Cell(r, headers["Satuan"]).GetString(),
                        });
                    }

                    r++;
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            var brands = importedItems.Select(i => i.Brand).Distinct().ToList();
            var codes = importedItems.Select(i => i.Code).Distinct().ToList();
            var types = importedItems.Select(i => i.Type).Distinct().ToList();
            var units = importedItems.Select(i => i.Unit).Distinct().ToList();

            var brandsDict = await _db.Brands
                .Where(q => brands.Contains(q.Name))
                .ToDictionaryAsync(q => q.Name, q => q.Id, cancellationToken);

            var typesDict = await _db.ItemTypes
                .Where(q => types.Contains(q.Name))
                .ToDictionaryAsync(q => q.Name, q => q.Id, cancellationToken);

            var unitsDict = await _db.ItemUnits
                .Where(q => units.Contains(q.Name))
                .ToDictionaryAsync(q => q.Name, q => q.Id, cancellationToken);

            var existingItems = await _db.Items
                .Where(q => codes.Contains(q.Code) && q.WarehouseId == WarehouseEnums.Balikpapan)
                .ToListAsync(cancellationToken);

            var itemsNotMentioned = await _db.Items
                .Where(q => !codes.Contains(q.Code) && q.WarehouseId == WarehouseEnums.Balikpapan)
                .ToListAsync(cancellationToken);

            var newBrands = importedItems
                .Select(q => q.Brand)
                .Where(q => !brandsDict.ContainsKey(q))
                .Distinct()
                .Select(name => new Brand { Name = name })
                .ToList();

            _db.Brands.AddRange(newBrands);

            var newTypes = importedItems
                .Select(q => q.Type)
                .Where(q => !typesDict.ContainsKey(q))
                .Distinct()
                .Select(name => new ItemType { Name = name })
                .ToList();

            _db.ItemTypes.AddRange(newTypes);

            var newUnits = importedItems
                .Select(q => q.Unit)
                .Where(q => !unitsDict.ContainsKey(q))
                .Distinct()
                .Select(name => new ItemUnit { Name = name })
                .ToList();

            _db.ItemUnits.AddRange(newUnits);

            await _db.SaveChangesAsync(cancellationToken);
            foreach (var type in newTypes)
            {
                typesDict[type.Name] = type.Id;
            }

            foreach (var brand in newBrands)
            {
                brandsDict[brand.Name] = brand.Id;
            }

            foreach (var unit in newUnits)
            {
                unitsDict[unit.Name] = unit.Id;
            }

            var existingCodes = existingItems.Select(q => q.Code).ToHashSet();
            var itemsToInsert = importedItems
                .Where(q => !existingCodes.Contains(q.Code))
                .ToList();

            foreach (var item in existingItems)
            {
                var importedItem = importedItems.FirstOrDefault(q => item.Code.Contains(q.Code));
                if (importedItem != null)
                {
                    item.Quantity = importedItem.Quantity;
                    item.UpdatedAt = DateTime.UtcNow;
                }
            }

            foreach (var item in itemsNotMentioned)
            {
                item.Quantity = 0;
                item.UpdatedAt = DateTime.UtcNow;
            }

            rowsUpdated += existingItems.Count + itemsNotMentioned.Count;

            var newItems = itemsToInsert
                .Select(item => new Item
                {
                    BrandId = brandsDict[item.Brand],
                    UnitId = unitsDict[item.Unit],
                    TypeId = typesDict[item.Type],
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Code = item.Code,
                    WarehouseId = WarehouseEnums.Balikpapan
                }).ToList();

            if (newItems.Count > 0)
            {
                _db.Items.AddRange(newItems);
                rowsInserted += newItems.Count;
            }

            await _db.SaveChangesAsync(cancellationToken);
            await _service.DeleteAsync(blobName);
        }

        return new ItemsResponse
        {
            IsSuccess = true,
            Message = $"Successfully imported items ({rowsUpdated} rows updated, {rowsInserted} rows inserted)"
        };
    }

    private int GetStartingIndexHelper(IXLWorksheet sheet)
    {
        var candidates = new[]
        {
            "Kode Item",
            "Kd. Item"
        };

        foreach (var candidate in candidates)
        {
            var result = FindCellByLabel(sheet, candidate);
            if (result != null)
            {
                return result.WorksheetRow().RowNumber();
            }
        }

        return 0;
    }

    private IXLCell? FindCellByLabel(IXLWorksheet sheet, string label)
    {
        foreach (var cell in sheet.CellsUsed())
        {
            if (cell.GetString().Contains(label, StringComparison.OrdinalIgnoreCase))
            {
                return cell;
            }
        }

        return null;
    }
}