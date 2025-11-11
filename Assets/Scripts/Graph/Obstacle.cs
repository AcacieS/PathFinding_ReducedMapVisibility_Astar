using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private List<Vertex> vertices = new List<Vertex>();
    public List<Edge> edges = new List<Edge>();
    private bool finishNeighbour = false;
    public void Reset()
    {
        foreach (Vertex v in vertices)
        {
            v.Reset();
        }
        vertices = new List<Vertex>();
        edges = new List<Edge>();
        finishNeighbour = false;
    }

    public List<Vertex> getVertices()
    {
        return vertices;
    }

    public void BuildVertices()
    {
        Debug.Log(" ----------- Test cost---------");
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Vertex"))
            {

                Vertex newV = new Vertex(gameObject, child, GameManager.Instance.GetId(), child.gameObject.GetComponent<VertexProp>().type);
                vertices.Add(newV);
                //NEED HERE
                // newV.SetCostVertex();
                child.gameObject.GetComponent<VertexProp>().SetVertex(newV);
                Debug.Log(newV);
            }
        }
    }
    public List<Edge> BuildEdges()
    {

        Reset();
        BuildVertices();


        for (int i = 0; i < vertices.Count; i++)
        {

            Vertex start = vertices[i];
            Vertex end = vertices[(i + 1) % vertices.Count];
            Edge edgeSE = new Edge(start, end);
            Edge edgeES = new Edge(end, start);

            start.AddNeighbor(end);
            end.AddNeighbor(start);
            edges.Add(edgeSE);
            edges.Add(edgeES);
        }
        finishNeighbour = true;
        ExtendVertex();
        return edges;

    }
    
    private void ExtendVertex()
    {
        for (int i = 0; i < vertices.Count; i++)
        {

            vertices[i].ExtendVertex();
        }

    }
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        if (finishNeighbour)
        {
            foreach(Vertex vertex in vertices)
            {
                if (vertex.GetBisectorDir() != Vector3.zero)
                {
                    
                    Gizmos.DrawLine(vertex.getPos(), vertex.getPos()+vertex.GetBisectorDir()*1);
                }
            }
        }
        
        
    }

}
