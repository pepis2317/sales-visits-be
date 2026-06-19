using System.Runtime.InteropServices.JavaScript;

namespace sales_visits_be.Models.Locations;

public class GetVisitPlanDayResponse
{
    public List<VisitPlanData> Plans { get; set; } = new();
}
public class VisitPlanData
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; }
    public DateTime Date { get; set; }
    public int VisitOrder { get; set; }
}