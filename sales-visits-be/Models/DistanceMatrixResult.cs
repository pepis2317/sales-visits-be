namespace sales_visits_be.Models;

public class DistanceMatrixResult
{
    public List<CustomerPriorityScore> Customers { get; set; }
    //[i,j] = distance in meters from customers[i] to customers[j]
    public long[,] Matrix { get; set; }
    public int Count => Customers.Count;
}