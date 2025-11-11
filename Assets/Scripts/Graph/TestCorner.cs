using System.Collections.Generic;
using UnityEngine;
public class TestCorner : MonoBehaviour
{
    public List<Transform> verticesT = new List<Transform>();
    public Transform BT;
    public List<Vertex> vertices = new List<Vertex>();
    public List<Edge> edges = new List<Edge>();
    private void Start()
    {
        int i = 0;
        foreach (Transform vertexT in verticesT)
        {
            Vertex vertex = new Vertex(gameObject, vertexT, i);
            vertices.Add(vertex);
            i++;
        }
        BuildEdges();
        TestVertex();
        Test2Vertex();
        //Bisector();
        //AnotherLine();
    }
    private void TestVertex()
    {
        foreach (Vertex vertex in vertices)
        {
            Debug.Log(vertex);
        }

    }
    public List<Edge> BuildEdges()
    {
        for (int i = 0; i < vertices.Count; i++)
        {

            Vertex start = vertices[i];
            Vertex end = vertices[(i + 1) % vertices.Count];
            Edge edge = new Edge(start, end);
            start.AddNeighbor(end);
            end.AddNeighbor(start);
            edges.Add(edge);
        }
        return edges;
    }
    public void Test2Vertex()
    {
        Vertex B = new Vertex(gameObject, BT, 10); ;
        Vertex A = vertices[2];
        A.SetBisectorDir();
        Debug.Log("isBitangent? " + A.IsBitangent(B));
    }
    
    
    // private void OnDrawGizmos()
    // {

    //     Gizmos.color = Color.cyan;
    //     if (bisectorEnd != Vector3.zero)
    //     {
    //         Gizmos.DrawLine(vertices[2].getPos(), bisectorEnd);
    //     }
    //     if (otherEnd != Vector3.zero)
    //     {
    //         Gizmos.color = Color.magenta;
    //         Gizmos.DrawLine(vertices[2].getPos(), otherEnd);
    //     }
        
    // }

}