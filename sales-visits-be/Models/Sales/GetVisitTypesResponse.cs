namespace sales_visits_be.Models.VisitTypes;

public class GetVisitTypesResponse
{
    public List<Dropdown> Types { get; set; } = new();
}