using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class AStarPathfinder
{
    public static List<Vertex> FindPath(Graph graph, Vertex start, Vertex goal)
    {
        var openSet = new List<Vertex> { start };
        var cameFrom = new Dictionary<Vertex, Vertex>();

        // gScore cost from start to vertex
        var gScore = new Dictionary<Vertex, float>();
        // fScore estimated tot cost (g + heuristic)
        var fScore = new Dictionary<Vertex, float>();

        foreach (var v in graph.vertices)
        {
            gScore[v] = float.PositiveInfinity;
            fScore[v] = float.PositiveInfinity;
        }

        gScore[start] = 0f;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            // Vertex in openSet lowest fScore
            Vertex current = openSet.OrderBy(v => fScore[v]).First();

            // If reached goal, get path
            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (Edge edge in current.GetNeighbor())
            {
                Vertex neighbor = edge.GetOtherVertex(current);
                if (neighbor == null) continue;

                float tentativeG = gScore[current] + edge.GetWeight();
                
                if (tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // If no path found
        return null;
    }

    private static float Heuristic(Vertex a, Vertex b)
    {
        return Vector3.Distance(a.getPos(), b.getPos());
    }

    private static List<Vertex> ReconstructPath(Dictionary<Vertex, Vertex> cameFrom, Vertex current)
    {
        var totalPath = new List<Vertex> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }
}