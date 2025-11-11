using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    int ID;
    List<Vector2> vertices;
    public Polygon(int pId, List<Vector2> pVertices)
    {
        ID = pId;
        vertices = pVertices;
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
        

}
