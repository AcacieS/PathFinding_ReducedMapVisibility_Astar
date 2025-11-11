using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class RUG : MonoBehaviour
{
    private List<Edge> edges = new List<Edge>();
    private List<Edge> sameObjEdges = new List<Edge>();
    private List<Vertex> allVertices = new List<Vertex>();
    private List<Edge> testStartGoal = new List<Edge>();
    private List<Vertex> pathToDraw = new List<Vertex>();
    private List<Vertex> pathToDrawHeuristic = new List<Vertex>(); 
    private const float EPSILON = 0.05f;
    
    public void GetAllVertices()
    {
        
    }
    public void BuildRUG(List<GameObject> obstacleObjs, Vertex start, Vertex goal)
    {
        Reset();
        SameObstacleEdges(obstacleObjs);
        BitangentEdges();
        if (!GameManager.Instance.isTestingRVGMap)
        {
            TestAStar(start, goal);
        }
    }
    private void Reset()
    {
        pathToDraw = new List<Vertex>();
        pathToDrawHeuristic = new List<Vertex>();
        edges = new List<Edge>();
        sameObjEdges = new List<Edge>();
        allVertices = new List<Vertex>();
        testStartGoal = new List<Edge>();
        ResetHeuristic(g);
    }

    private void SameObstacleEdges(List<GameObject> obstacleObjs)
    {
        sameObjEdges.Clear();

        foreach (GameObject obstacle in obstacleObjs)
        {
            var obstacleEdges = obstacle.GetComponent<Obstacle>().BuildEdges();
            sameObjEdges.AddRange(obstacleEdges);
            allVertices.AddRange(obstacle.GetComponent<Obstacle>().getVertices());
        }

        Debug.Log($"Same-object edges: {sameObjEdges.Count}");
    }
    private void BitangentEdges()
    {
        Bitangent();
    }
    private void Bitangent()
    {
        for (int i = 0; i < allVertices.Count; i++)
        {
            for (int j = i + 1; j < allVertices.Count; j++)
            {
                Vertex a = allVertices[i];
                Vertex b = allVertices[j];

                // Skip same-object edges and same tangent if a -> b
                if (a.SameGroup(b) || ContainsEdge(a, b))
                    continue;

                if (IsBitangent(a, b) && IsVisible(a, b))
                {
                    Edge edgeAB = new Edge(a, b);
                    Edge edgeBA = new Edge(b, a);
                    a.AddNeighbor(b);
                    b.AddNeighbor(a);
                    edges.Add(edgeAB);
                    edges.Add(edgeBA);
                }


            }
        }

        Debug.Log($"Total edges bitangent: {edges.Count}");
        // Combine with same-object edges
        edges.AddRange(sameObjEdges);
        Debug.Log($"Total edges (with bitangent): {edges.Count}");

    }
    
    private bool IsVisible(Vertex a, Vertex b)
    {
        Vector2 A = a.getPosXZ();
        Vector2 B = b.getPosXZ();

        foreach (Edge edge in sameObjEdges)
        {
            Vector2 C = edge.GetA().getPosXZ();
            Vector2 D = edge.GetB().getPosXZ();

            // Don’t check against the obstacle edges that share a vertex with A or B
            if (a.IsSame(edge.GetA()) || a.IsSame(edge.GetB()) ||
                b.IsSame(edge.GetA()) || b.IsSame(edge.GetB()))
                continue;

            if (LineSegmentsIntersect(A, B, C, D))
            {
                return false;
            }
        }

        return true; // no intersection with any obstacle edge
    }

    private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d1 = Cross(p2 - p1, p3 - p1);
        float d2 = Cross(p2 - p1, p4 - p1);
        float d3 = Cross(p4 - p3, p1 - p3);
        float d4 = Cross(p4 - p3, p2 - p3);

        // Proper intersection
        if (((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0)) &&
            ((d3 > 0 && d4 < 0) || (d3 < 0 && d4 > 0)))

            return true;

        // Collinear and overlapping
        if (Mathf.Approximately(d1, 0) && OnSegment(p1, p3, p2)) return true;
        if (Mathf.Approximately(d2, 0) && OnSegment(p1, p4, p2)) return true;
        if (Mathf.Approximately(d3, 0) && OnSegment(p3, p1, p4)) return true;
        if (Mathf.Approximately(d4, 0) && OnSegment(p3, p2, p4)) return true;

        return false;
    }
    private bool LineSegmentsIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
{
    intersection = Vector2.zero;

    Vector2 r = p2 - p1;
    Vector2 s = p4 - p3;

    float rxs = Cross(r, s);
    float qpCrossR = Cross(p3 - p1, r);

    // Parallel lines
    if (Mathf.Abs(rxs) < 1e-6f)
        return false;

    float t = Cross(p3 - p1, s) / rxs;
    float u = Cross(p3 - p1, r) / rxs;

    if (t >= 0f && t <= 1f && u >= 0f && u <= 1f)
    {
        intersection = p1 + t * r; // This is the true intersection point
        return true;
    }

    return false;
}
private float Cross(Vector2 a, Vector2 b)
{
    return a.x * b.y - a.y * b.x;
}

private bool OnSegment(Vector2 a, Vector2 b, Vector2 c)
{
    return b.x >= Mathf.Min(a.x, c.x) && b.x <= Mathf.Max(a.x, c.x) && b.y >= Mathf.Min(a.y, c.y) && b.y <= Mathf.Max(a.y, c.y);
}

    private bool IsBitangent(Vertex a, Vertex b)
    {
        a.SetBisectorDir();
        bool isBitangentA = a.IsBitangent(b);
        b.SetBisectorDir();
        bool isBitangentB = b.IsBitangent(a);
        // Debug.Log("isBitangent? " + a.IsBitangent(b));
        return isBitangentA && isBitangentB;
    }


    private bool ContainsEdge(Vertex a, Vertex b)
    {
        foreach (var e in edges)
        {
            if (e.IsSame(a, b) || e.IsSame(b, a))
                return true;
        }
        return false;
    }
    private Graph g = new Graph();
    public void TestAStar(Vertex start, Vertex goal)
    {
        
        g = new Graph();
        g.vertices = allVertices;
        g.edges = edges;

        g.vertices.Add(start);
        g.vertices.Add(goal);
        foreach (Vertex v in allVertices)
        {
            if (IsVisible(start, v))
            {
                Edge e = new Edge(start, v);
                Edge e2 = new Edge(v, start);
                start.AddNeighbor(v);
                v.AddNeighbor(start);
                g.edges.Add(e);
                g.edges.Add(e2);
                testStartGoal.Add(e);
            }

            if (IsVisible(goal, v))
            {
                Edge e = new Edge(goal, v);
                Edge e2 = new Edge(v, goal);
                goal.AddNeighbor(v);
                v.AddNeighbor(goal);
                g.edges.Add(e);
                g.edges.Add(e2);
                testStartGoal.Add(e);
            }
        }
        CostEdges();

        // foreach (Vertex v in g.vertices)
        // {
        //     Debug.Log(v);
        // }

        List<Vertex> path = AStarPathfinder.FindPath(g, start, goal);

        if (path != null)
        {
            Debug.Log("Path found: " + string.Join(" -> ", path.Select(v => v.ToString())));
            Debug.LogWarning("Should Move RUG");
            DrawPath(path);
            if (GameManager.Instance.isTestingHeuristic)
            {
                AddIntersectionVertices(g);
                path = AStarPathfinder.FindPath(g, start, goal);
                pathToDrawHeuristic = path;
            }
            
            PathLengthTest.TestPathLength(path);
            
            
            if (!GameManager.Instance.isTestingRVGStartGoal)
            {
                GameManager.Instance.GetCurrentMA().Move(path);
            }

        }
        else
        {
            Debug.LogWarning("No path found.");
        }
    }

    private void CostEdges()
    {
        foreach (Edge edge in edges)
        {
            if (edge.GetA().GetID() == 0 || edge.GetB().GetID() == 0)
            {
                Debug.LogWarning("has 0");
            }
            SetCostEdges(edge);
        }
    }
    private void SetCostEdges(Edge current)
    {
        float weight = 0f;

        Dictionary<Edge, Vector2> intersections = new Dictionary<Edge, Vector2>();

        Debug.Log("intersection " + current.GetA().GetID() + " -> " + current.GetB().GetID() + ": ");
        Debug.Log("======= SEARCH INTERSECTION");
        Debug.Log("edge A: " + current.GetA()+"\n"+"edge B: "+current.GetB());
        foreach (var costEdge in Cost.InstanceC.GetAreaEdges())
        {
            // if (current.GetWeight() != 0) continue;
            if (LineSegmentsIntersect(current.GetA().getPosXZ(), current.GetB().getPosXZ(), costEdge.GetA().getPosXZ(), costEdge.GetB().getPosXZ(), out Vector2 inter))
            {
                Debug.Log("inter: " + inter);
                intersections.Add(costEdge, inter);
            }
        }
        Debug.Log("----------- FINISH --------");
        current.SetIntersection(intersections);
        float currentCost = current.GetA().GetCost();
        if (currentCost == 0)
        {
            Debug.LogError("cost not assigned yet" + currentCost);
        }
        Vector2 posPrev = current.GetA().getPosXZ();

        Debug.Log("========== SET WEIGHT ========== ");
        Debug.Log("A WHO IS " + current.GetA());
        Debug.Log("to B " + current.GetB());

        foreach (var intersection in intersections)
        {
            Edge edge = intersection.Key;
            Vector2 intersectPt = intersection.Value;
            float d = Distance(posPrev, intersectPt);
            weight += d * currentCost;

            posPrev = intersectPt;
            Debug.Log("weight : "+weight+ "at cost area: "+currentCost +", d:"  + d+ " interesectPt:"+intersectPt);
            currentCost = edge.GetCostOther(currentCost);
            Debug.Log("new cost" + currentCost);
            if(currentCost == 0)
            {
                Debug.Log("ERROR: 0");
            }
        }

        float last_d = Distance(posPrev, current.GetB().getPosXZ());
        weight += last_d * currentCost;
        Debug.Log("weight: "+weight+ "at cost area: "+currentCost);
        current.SetWeight(weight);
        
    }
    private float Distance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2) + Mathf.Pow(pos2.y - pos1.y, 2));
    }

    // ================================== SIMPLIFY OPTIMAL =================================
    /*
    Add new vertices and slit old edges
    
    
    */
    List<Vertex> newVertices = new List<Vertex>();
    List<Edge> edgesToRemove = new List<Edge>();
    List<Edge> edgesToAdd = new List<Edge>();
    List<Edge> edgesToAddV = new List<Edge>();
    [SerializeField] private GameObject vertexPrefab;
    [SerializeField] private Transform ground;
    private void ResetHeuristic(Graph rvg)
    {
        Debug.Log("RESET");
        edgesToRemove = new List<Edge>();
        edgesToAdd = new List<Edge>();
        foreach (Vertex v in newVertices)
        {
            foreach (Edge neighbor in v.GetNeighbor())
            {
                neighbor.GetOtherVertex(v).RemoveNeighbor(v); // make sure neighbor no longer references v
            }
            Destroy(v.getObj());
            rvg.vertices.Remove(v); // remove from the graph
        }
        newVertices.Clear();
            newVertices = new List<Vertex>();
            edgesToAddV = new List<Edge>();
        }
    public void AddIntersectionVertices(Graph rvg)
    {
        // Check all pairs of edges
        for (int i = 0; i < rvg.edges.Count; i++)
        {
            Edge e1 = rvg.edges[i];
            for (int j = i + 1; j < rvg.edges.Count; j++)
            {
                Edge e2 = rvg.edges[j];

                // Skip edges that share a vertex
                if (e1.GetA().IsSame(e2.GetA()) || e1.GetA().IsSame(e2.GetB()) || e1.GetB().IsSame(e2.GetA()) || e1.GetB().IsSame(e2.GetB()))
                    continue;

                if (LineSegmentsIntersect(e1.GetA().getPosXZ(), e1.GetB().getPosXZ(), e2.GetA().getPosXZ(), e2.GetB().getPosXZ(), out Vector2 intersection))
                {
                    GameObject newVObj = Instantiate(vertexPrefab, new Vector3(intersection.x, e2.GetA().getPos().y, intersection.y), Quaternion.identity);
                    // Create new vertex at intersection
                    newVObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    Vertex newV = new Vertex(this.gameObject, newVObj.transform);
                    newVertices.Add(newV);

                    //Split edge1
                    Edge newE1 = new Edge(e1.GetA(), newV);
                    Edge new1E = new Edge(newV, e1.GetA());
                    Edge newE2 = new Edge(newV, e1.GetB());
                    Edge new2E = new Edge(e1.GetB(), newV);

                    edgesToAdd.Add(newE1);
                    edgesToAdd.Add(new1E);
                    edgesToAdd.Add(newE2);
                    edgesToAdd.Add(new2E);
                    
                    edgesToRemove.Add(e1);

                    // Split edge2
                    edgesToAdd.Add(new Edge(e2.GetA(), newV));
                    edgesToAdd.Add(new Edge(newV, e2.GetA()));
                    edgesToAdd.Add(new Edge(newV, e2.GetB()));
                    edgesToAdd.Add(new Edge( e2.GetB(), newV));



                    edgesToRemove.Add(e2);

                    // Update neighbors
                    e1.GetA().UpdateNeighbor(newV, e1.GetB());
                    e1.GetB().UpdateNeighbor(newV, e1.GetA());
                    e2.GetA().UpdateNeighbor(newV, e2.GetB());
                    e2.GetB().UpdateNeighbor(newV, e2.GetA());
                }
            }
        }

        // Remove old edges
        foreach (Edge e in edgesToRemove)
            rvg.edges.Remove(e);
        // Add new edges
        rvg.edges.AddRange(edgesToAdd);

        // Add new vertices
        rvg.vertices.AddRange(newVertices);

        //BuildNewEdges();

        // Add new edges
        //rvg.edges.AddRange(edgesToAddV);

        Debug.Log($"Added {newVertices.Count} intersection vertices, split {edgesToRemove.Count} edges.");
    }

    // -------------------------------------- HELPER ---------------
    private void DrawPath(List<Vertex> path)
    {
        pathToDraw = path; // store the path
    }

    private void OnDrawGizmos()
    {
        // Draw RUG edges if you want
        
        if (edges != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Edge e in edges)
                Gizmos.DrawLine(e.GetA().getPos(), e.GetB().getPos());
        }

        // Draw path found
        if (pathToDraw != null && pathToDraw.Count > 1)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < pathToDraw.Count - 1; i++)
            {
                Gizmos.DrawLine(pathToDraw[i].getPos(), pathToDraw[i + 1].getPos());
            }
        }
        //Draw path found
        if (pathToDrawHeuristic != null && pathToDraw.Count > 1)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < pathToDrawHeuristic.Count - 1; i++)
            {
                Gizmos.DrawLine(pathToDrawHeuristic[i].getPos(), pathToDraw[i + 1].getPos());
            }
        }

        Gizmos.color = Color.red;
        foreach (Edge edge in sameObjEdges)
        {
            Gizmos.DrawLine(edge.GetA().getPos(), edge.GetB().getPos());
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.isTestingHeuristic)
            {
                //OnDrawNew();
                
                int i = 0;
                foreach (Edge e in edgesToAddV)
                {
                    if (i % 2 == 0)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.magenta;
                    }
                    Gizmos.DrawLine(e.GetA().getPos(), e.GetB().getPos());
                    i++;
                }
                
        }
        }

    }

}