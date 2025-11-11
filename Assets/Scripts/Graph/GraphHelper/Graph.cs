using System.Collections.Generic;

public class Graph
{
    public List<Vertex> vertices;
    public List<Edge> edges;

    public Graph()
    {
        vertices = new List<Vertex>();
        edges = new List<Edge>();
    }
    public Vertex AddVertex(Vertex v)
    {
        vertices.Add(v);
        return v;
    }
    


}