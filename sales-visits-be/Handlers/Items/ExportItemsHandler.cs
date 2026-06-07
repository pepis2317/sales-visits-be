using ClosedXML.Excel;
using entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using sales_visits_be.Models.Items;

namespace sales_visits_be.Handlers.Items;

public class ExportItemsHandler : IRequestHandler<ExportItemsRequest, ExportItemsResponse>
{
    private readonly ApplicationDbContext _db;

    public ExportItemsHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ExportItemsResponse> Handle(ExportItemsRequest request, CancellationToken cancellationToken)
    {
        var warehouse = await _db.Warehouses.FirstOrDefaultAsync(q => q.Id == request.WarehouseId, cancellationToken);
        var brands = await _db.Brands.ToListAsync(cancellationToken);
        var items = await _db.Items.Include(q => q.Type)
            .Where(q => q.WarehouseId == request.WarehouseId)
            .Select(q => new ExportItemDTO
            {
                NewJis = q.Code,
                Group = q.Type != null ? q.Type.Name : "-",
                Type = q.ShortName ?? q.Name,
                Quantity = q.Quantity,
                BrandId = q.BrandId
            }).OrderBy(q => q.Group).ThenBy(q => q.Type).ToListAsync(cancellationToken);

        var brandIds = items.Select(q => q.BrandId).Where(q => q != null).ToList().Distinct();
        var wb = new XLWorkbook();
        foreach (var brandId in brandIds)
        {
            var brand = brands.FirstOrDefault(q => q.Id == brandId);
            var itemsFiltered = items.Where(q => q.BrandId == brandId).ToList();
            var ws = wb.Worksheets.Add(brand.Name);
            GenerateHeader(ws);
            GenerateRows(ws, itemsFiltered);
            var lastRow = itemsFiltered.Count + 2;
            ws.Cell(1, 4).FormulaA1 = $"=SUM(D3:D{lastRow})";

            var usedRange = ws.RangeUsed();
            usedRange?.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Black)
                .Border.SetInsideBorderColor(XLColor.Black);

            ws.Columns().AdjustToContents();
        }

        var stream = new MemoryStream();
        wb.SaveAs(stream);
        stream.Position = 0;

        return new ExportItemsResponse
        {
            Stream = stream,
            ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            FileName = $"items_stock_{warehouse.Name}.xlsx"
        };
    }

    private void GenerateHeader(IXLWorksheet worksheet)
    {
        var headerRow = 2;
        var currentColumn = 1;
        var info = new[]
        {
            "NEW JIS",
            "GROUP",
            "TYPE",
            "QTY"
        };
        foreach (var title in info)
        {
            worksheet.Cell(headerRow, currentColumn++).Value = title;
        }

        var headerRange = worksheet.Range(headerRow, 1, headerRow, currentColumn - 1);

        headerRange.Style
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#F28C28"))
            .Font.SetBold(true);
    }

    private void GenerateRows(IXLWorksheet worksheet, List<ExportItemDTO> items)
    {
        var dataRow = 3;
        foreach (var item in items)
        {
            worksheet.Cell(dataRow, 1).Value = item.NewJis;
            worksheet.Cell(dataRow, 2).Value = item.Group;
            worksheet.Cell(dataRow, 3).Value = item.Type;
            worksheet.Cell(dataRow, 4).Value = item.Quantity;
            worksheet.Cell(dataRow, 4).Style.NumberFormat.Format = "#,##0.##;-#,##0.##;\"-\"";

            var dataRange = worksheet.Range(dataRow, 1, dataRow, 4);

            dataRange.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            dataRow++;
        }
    }
}