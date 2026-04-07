using Google.OrTools.ConstraintSolver;
using sales_visits_be.Models;

namespace sales_visits_be.Service;

public class RouteSolverService
{
    private const int TimeLimitSeconds = 10;

    public RouteResult Solve(DistanceMatrixResult input)
    {
        if (input.Count == 0)
        {
            return new RouteResult
            {
                SolutionFound = false,
                OrderedStops = []
            };
        }

        if (input.Count == 1)
        {
            return new RouteResult
            {
                SolutionFound = true,
                OrderedStops = input.Customers,
                TotalDistanceMeters = 0
            };
        }

        // one salesperson, depot at index 0.
        //depot is the starting and ending point of the route
        var manager = new RoutingIndexManager(
            input.Count,
            1,
            0
        );

        var routing = new RoutingModel(manager);

        int transitCallbackIndex = routing.RegisterTransitCallback((fromIndex, toIndex) =>
        {
            int from = manager.IndexToNode(fromIndex);
            int to = manager.IndexToNode(toIndex);
            return input.Matrix[from, to];
        });
        
        routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

        //PATH_CHEAPEST_ARC builds initial solution greedily using nearest neighbor, the solver then improves within time limit
        var searchParams = operations_research_constraint_solver.DefaultRoutingSearchParameters();
        searchParams.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
        searchParams.TimeLimit = new Google.Protobuf.WellKnownTypes.Duration
        {
            Seconds = TimeLimitSeconds
        };
        //solve
        var solution = routing.SolveWithParameters(searchParams);
        if(solution == null)
        {
            return new RouteResult
            {
                SolutionFound = false,
                OrderedStops = []
            };
        }

        var orderedStops = new List<CustomerPriorityScore>();
        long totalDistance = 0;

        var index = routing.Start(0);
        while(!routing.IsEnd(index))
        {
            int node = manager.IndexToNode(index);
            orderedStops.Add(input.Customers[node]);

            var nextIndex = solution.Value(routing.NextVar(index));
            totalDistance += input.Matrix[node, manager.IndexToNode(nextIndex)];
            index = nextIndex;
        }

        return new RouteResult
        {
            SolutionFound = true,
            OrderedStops = orderedStops,
            TotalDistanceMeters = totalDistance
        };
    }
}