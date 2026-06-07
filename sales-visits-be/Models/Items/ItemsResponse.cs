using MediatR;

namespace sales_visits_be.Models.Items;

public class ItemsResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}