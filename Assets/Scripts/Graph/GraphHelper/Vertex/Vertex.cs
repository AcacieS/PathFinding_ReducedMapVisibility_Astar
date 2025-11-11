
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    private int id;
    private GameObject group;
    private Transform vertexPos;
    private List<Edge> neighbors;
    private Vector3 bisectorDir;
    private VertexType type;
    private bool isExtend;
    private bool isSetBisector = false;
    private Vector3 oldVertex;
    private float size = 0f;
    private float cost;

    public Vertex(GameObject pGroup, Transform pVertexPos)
    {
        group = pGroup;
        vertexPos = pVertexPos;
        neighbors = new List<Edge>();
        isExtend = false;
        id = GameManager.Instance.GetId();
        Debug.Log("id set is " + id);
        SetCostVertex();
    }
    public float Distance(Vector2 pos)
    {
        return Mathf.Sqrt(Mathf.Pow(pos.x - getPosXZ().x, 2) + Mathf.Pow(pos.y - getPosXZ().y, 2));
    }
    public Vertex()
    {
        
    }
    public Vertex(GameObject pGroup, Transform pVertexPos, int pId)
    {
        group = pGroup;
        vertexPos = pVertexPos;
        neighbors = new List<Edge>();
        id = pId;
        SetCostVertex();
    }
    public Vertex(GameObject pGroup, Transform pVertexPos, int pId, VertexType pType)
    {
        group = pGroup;
        vertexPos = pVertexPos;
        neighbors = new List<Edge>();
        id = pId;
        type = pType;
        SetCostVertex();
    }
    float tolerance = 0.05f; // small worlds (1 unit ~ 1 meter)

    private void SetCostVertex()
    {
        //SUBAREA
        Vector2 pt = this.getPosXZ();
        foreach (GameObject subArea in Cost.InstanceC.GetSubAreas())
        {
            List<Vector2> verticesXZ = subArea.GetComponent<SubArea>().GetVerticesXZ();
            bool isIn = IsPointInPolygon(pt, verticesXZ);
            bool isNear = IsPointNearPolygon(pt, verticesXZ, tolerance);
            if (isIn || isNear)
            {
                SetCost(subArea.GetComponent<SubArea>().GetCost());
                return;
            }
        }
        Debug.LogWarning("ERROR should got inside a subArea");
    }
    private bool IsPointNearPolygon(Vector2 point, List<Vector2> polygon, float tolerance)
    {
        int count = polygon.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % count];

            // Check near any vertex
            if (Vector2.Distance(point, a) <= tolerance || Vector2.Distance(point, b) <= tolerance)
                return true;

            // Check near any edge
            float dist = DistancePointToSegment(point, a, b);
            if (dist <= tolerance)
                return true;
        }
        return false;
    }
private float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
{
    Vector2 ab = b - a;
    float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
    t = Mathf.Clamp01(t);
    Vector2 projection = a + t * ab;
    return Vector2.Distance(p, projection);
}
    public bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
    {
        bool inside = false;
        int count = polygon.Count;

        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            Vector2 vi = polygon[i];
            Vector2 vj = polygon[j];

            bool intersect = ((vi.y > point.y) != (vj.y > point.y)) &&
                            (point.x < (vj.x - vi.x) * (point.y - vi.y) / (vj.y - vi.y + Mathf.Epsilon) + vi.x);

            if (intersect)
                inside = !inside;
        }

        return inside;
    }
    public float GetCost()
    {
        return cost;
    }
    public void SetCost(float pCost)
    {
        //Debug.Log("Set Cost: " + pCost);
        cost = pCost;
    }
    public void SetBisectorDir()
    {
        if (isSetBisector) return;
        List<Vector2> neigh_dirs = new List<Vector2>();
        foreach (Edge neighbour in neighbors)
        {
            neigh_dirs.Add(neighbour.GetDir());
        }
        bisectorDir = GetAngleBisector(neigh_dirs[0], neigh_dirs[1]);
        if (type == VertexType.Interior)
        {
            bisectorDir = -bisectorDir;
        }
        isSetBisector = true;
    }
    public Vector3 GetBisectorDir()
    {
        if (!isSetBisector)
        {
            SetBisectorDir();
        }
        return bisectorDir;
    }
    public bool IsBitangent(Vertex other)
    {
        Vector3 otherPos = getPos() + GetDir2Vertices(other);
        Vector3 otherDir = (otherPos - getPos()).normalized;

        float angle = Vector3.Angle(bisectorDir, otherDir);
        //Debug.Log($"Angle between bisector and other line: {angle}°");
        if (angle <= 45f || angle >= (180-45))
        {
            return false;
        }
        return true;
    }
    public Vector3 GetDir2Vertices(Vertex other)
    {
        Vector3 v1 = new Vector3(getPos().x - other.getPos().x, 0, getPos().z - other.getPos().z);

        return v1;
    }
    public void Reset()
    {
        isExtend = false;
        ExtendVertex(-1);
    }
    public void ExtendVertex(int reset = 1)
    {
        if (isExtend) return;

        if (!isSetBisector)
        {
            SetBisectorDir();
        }
        if (size == 0f)
        {
            size = GameManager.Instance.GetCurrentMA().GetInfo().size;
        }

        
        //Debug.Log("----- current size: " + size + " reset = "+reset);
        vertexPos.position = vertexPos.position - reset*bisectorDir * size/2;
        isExtend = true;

    }
    private Vector3 GetAngleBisector(Vector2 v1, Vector2 v2)
    {
        Vector2 normalizedV1 = v1.normalized;
        Vector2 normalizedV2 = v2.normalized;

        Vector2 bisector2D = (normalizedV1 + normalizedV2).normalized;
        return new Vector3(bisector2D.x, 0, bisector2D.y);
    }
    public void RemoveNeighbor(Vertex v)
    {
        List<Edge> neighbours_removed = new List<Edge>();
        foreach (Edge edge in neighbors)
        {
            if (edge.HasV(v))
            {
                neighbours_removed.Add(edge);
            }
        }
        foreach (Edge edge in neighbours_removed)
        {
                neighbors.Remove(edge);
        }
        
    }
    
    public List<Edge> GetNeighbor()
    {
        return neighbors;
    }
    public bool AddNeighbor(Vertex v, float weight)
    {

        Edge newEdge = new Edge(this, v, weight);
        neighbors.Contains(newEdge);
        foreach (Edge edge in neighbors)
        {
            if (newEdge.IsSame(edge))
            {
                return false;
            }
        }
        neighbors.Add(newEdge);
        return true;
    }
    public void UpdateNeighbor(Vertex newV, Vertex oldV)
    {
        AddNeighbor(newV);
        RemoveNeighbor(oldV);
    }
    public bool AddNeighbor(Vertex v)
    {
        Edge newEdge = new Edge(this, v);
        neighbors.Contains(newEdge);
        foreach (Edge edge in neighbors)
        {
            if (newEdge.IsSame(edge))
            {
                return false;
            }
        }
        neighbors.Add(newEdge);
        return true;
    }

    public Vector2 getPosXZ()
    {
        return new Vector2(vertexPos.position.x, vertexPos.position.z);
    }
    public Transform getTransform()
    {
        return vertexPos;
    }
    public Vector3 getPos()
    {
        return vertexPos.position;
    }
    public GameObject getGroup()
    {
        return group;
    }
    public GameObject getObj()
    {
        return vertexPos.gameObject;
    }
    public bool SameGroup(Vertex other)
    {
        if (other.getGroup() == group)
        {
            return true;
        }
        return false;
    }
    public bool IsSame(Vertex other)
    {
        if (other.getTransform() == vertexPos)
        {
            return true;
        }
        return false;
    }
    public int GetID()
    {

        return id;
    }
    public override string ToString()
    {
        string neighborsIds = neighbors.Count == 0 
            ? "None" 
            : string.Join(", ", neighbors.ConvertAll(e => e.GetOtherVertex(this)?.GetID().ToString() ?? "null"));

        return $"Vertex ID: {id} with cost {cost}, Group: {group.name}, Pos: {getPosXZ()} Neighbors: [{neighborsIds}]";
    }
    
}