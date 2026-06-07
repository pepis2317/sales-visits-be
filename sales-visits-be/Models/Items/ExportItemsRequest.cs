using MediatR;

namespace sales_visits_be.Models.Items;

public class ExportItemsRequest:IRequest<ExportItemsResponse>
{
    public Guid WarehouseId { get; set; }
}