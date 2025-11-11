using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cost : MonoBehaviour
{
    public static Cost InstanceC { get; private set; }
    [SerializeField] private Material[] matsCost;
    [SerializeField] private GameObject[] subAreas;
    [SerializeField] private float[] speeds;
    private float minCost = 0.5f;
    private float maxCost = 5.0f;
    [SerializeField] private List<Transform> verticesT = new List<Transform>();
    List<Vertex> vertices = new List<Vertex>();
    public static List<Edge> edges = new List<Edge>();
    Dictionary<float, GameObject> costs = new Dictionary<float, GameObject>();
    public void SetCostInstance()
    {
        if (InstanceC == null)
        {
            Debug.Log("COST MANUAL Awake running!");
            InstanceC = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Awake()
    {
        if (InstanceC == null)
        {
            Debug.Log("COST Awake running!");
            InstanceC = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (InstanceC != this)
        {
            Destroy(gameObject);
        }
    }
    public GameObject[] GetSubAreas()
    {
        return subAreas;
    }
    public List<Edge> GetAreaEdges()
    {
        return edges;
    }
    private void BuildVertices()
    {
        foreach (Transform child in verticesT)
        {
            Vertex newV = new Vertex(gameObject, child, GameManager.Instance.GetId(), child.gameObject.GetComponent<VertexProp>().type);
            vertices.Add(newV);
        }
    }
    private void BuildEdges()
    {
        Vertex A14 = vertices[1];
        Vertex B14 = vertices[4];
        ManualBuildEdges(A14, B14, GetCost2(4, 5));

        Vertex A34 = vertices[3];
        Vertex B34 = vertices[4];
        ManualBuildEdges(A34, B34, GetCost2(4, 3));

        Vertex A45 = vertices[4];
        Vertex B45 = vertices[5];
        ManualBuildEdges(A45, B45, GetCost2(5, 2));

        Vertex A47 = vertices[4];
        Vertex B47 = vertices[7];
        ManualBuildEdges(A47, B47, GetCost2(3, 2));

        Vertex A78 = vertices[7];
        Vertex B78 = vertices[8];
        ManualBuildEdges(A78, B78, GetCost2(3, 0));

        Vertex A76 = vertices[7];
        Vertex B76 = vertices[6];
        ManualBuildEdges(A76, B76, GetCost2(2, 1));

        Vertex A6 = vertices[7];
        Vertex B6 = vertices[10];
        ManualBuildEdges(A6, B6, GetCost2(0, 1));

    }
    
    private void ManualBuildEdges(Vertex A, Vertex B, (float, float) cost)
    {
        //Debug.Log("A cost: " + A.GetCost() + ", B cost: " + B.GetCost());

        Edge edgeSE = new Edge(A, B);

        A.AddNeighbor(B);
        B.AddNeighbor(A);
        edgeSE.SetCost(cost);
        edges.Add(edgeSE);
    }

    private (float, float) GetCost2(int index1, int index2)
    {
        return (GetCost(index1), GetCost(index2));
    }

    private float GetCost(int index)
    {
        GameObject target = subAreas[index];
        float cost = costs.FirstOrDefault(kvp => kvp.Value == target).Key;
        return cost;
    }
    private void OnDrawGizmos()
    {
        // Draw RUG edges if you want
        if (edges.Count!=0)
        {
            Gizmos.color = Color.red;
            foreach (Edge e in edges)
            Gizmos.DrawLine(e.GetA().getPos(), e.GetB().getPos());
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    A -> B weight
    1. A pt Where am I? get cost of current point: area 1
    2. then check if the edge cross edge of area1.
    
    3. if yes: from here pt where am I?
    
    */
    public void SpawnCost()
    {
        Reset();
        Debug.Log("Spawn Cost");

        for (int i = 0; i < matsCost.Length; i++)
        {
            float cost = Random.Range(minCost, maxCost);
            foreach (float existingCost in costs.Keys)
            {
                while (Mathf.Abs(existingCost - cost) <= 0.1)
                {
                    cost = Random.Range(minCost, maxCost);
                }
            }
            subAreas[i].GetComponent<SubArea>().SetCost(cost);
            costs.Add(cost, subAreas[i]);
        }

        var sortedCosts = costs.OrderBy(pair => pair.Key).ToList();
        int e = 0;
        
        foreach (var kvp in sortedCosts)
        {
            kvp.Value.GetComponent<MeshRenderer>().material = matsCost[e];
            kvp.Value.GetComponent<SubArea>().SetSpeed(speeds[e]);
            e++;
        }

        BuildVertices();
        BuildEdges();

    }
     private void Reset()
    {
        costs = new Dictionary<float, GameObject>();
        edges = new List<Edge>();
        vertices = new List<Vertex>();

    }
    // ===================================================== OTHER FUNCTION =================================

}
