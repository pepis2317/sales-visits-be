using MediatR;
using sales_visits_be.Models;
using sales_visits_be.Models.Locations;
using sales_visits_be.Service;

namespace sales_visits_be.Handlers.Locations;

public class GenerateRouteHandler:IRequestHandler<GenerateRouteRequest, RouteResult>
{
    private readonly PriorityScoreService _scorer;
    private readonly DistanceMatrixService _matrix;
    private readonly RouteSolverService _solver;
    public GenerateRouteHandler(PriorityScoreService scorer, DistanceMatrixService matrix, RouteSolverService solver)
    {
        _scorer = scorer;
        _matrix = matrix;
        _solver = solver;
    }
    
    public async Task<RouteResult> Handle(GenerateRouteRequest request, CancellationToken cancellationToken)
    {
        var scored = await _scorer.GetScoredCustomersAsync(request.SalesId, request.MaxCustomers, cancellationToken);
        var matrix = _matrix.Build(scored, request.Latitude, request.Longitude);
        var result = _solver.Solve(matrix);
        result.OrderedStops.RemoveAt(0);
        return result;
    }
}