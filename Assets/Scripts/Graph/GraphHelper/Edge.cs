using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    private Vertex a;
    private Vertex b;
    private float weight;
    private Vector2 dir;

    public (float, float) costs;


    
    public Edge(Vertex pa, Vertex pb, float pweight)
    {
        a = pa;
        b = pb;
        weight = pweight;
        GetDir();
    }
    public Edge(Vertex pa, Vertex pb)
    {
        a = pa;
        b = pb;
        weight = 0;
    }
    public Vertex GetOtherVertex(Vertex v)
    {
        if (v.IsSame(a))
        {
            return b;
        }
        else
        {
            return a;
        }
    }
    public bool HasV(Vertex v)
    {
        if (v.IsSame(a)) return true;
        if (v.IsSame(b)) return true;
        return false;
    }
    Dictionary<Edge, Vector2> intersections = new Dictionary<Edge, Vector2>();
    public void SetIntersection(Dictionary<Edge, Vector2> pIntersections)
    {
        intersections = pIntersections;
    }
    public void SetCost((float, float) pCosts)
    {
        if(pCosts.Item1 == 0)
        {
            Debug.LogError("edge "+a.GetID()+"initial cost 0");
        }else if(pCosts.Item2 == 0)
        {
            Debug.LogError("edge "+a.GetID()+"initial cost 0");
        }
        costs = pCosts;
    }
    public float GetCostOther(float pCost)
    {
        if (costs.Item1 == pCost)
        {
            return costs.Item2;
        }
        else if (costs.Item2 == pCost)
        {
            return costs.Item1;
        }
        else
        {


        }
        Debug.LogError("didn't find cost " + pCost + ": " + costs);
        float diff1 = Mathf.Abs(costs.Item1 - pCost);
        float diff2 = Mathf.Abs(costs.Item2 - pCost);
        return diff1 < diff2 ? costs.Item1 : costs.Item2;
    }
    
    public void SetWeight(float pWeight)
    {
        weight = pWeight;
    }
    public float GetWeight()
    {
        return weight;
    }
    
    public Vertex GetA()
    {
        return a;
    }
    public Vertex GetB()
    {
        return b;
    }
    public Vector2 GetDir()
    {
        Vector2 v1 = new Vector2(b.getPos().x - a.getPos().x, b.getPos().z - a.getPos().z);
        dir = v1;
        return v1;
    }
    public bool IsSame(Vertex A, Vertex B)
    {
        if (A.IsSame(a) && B.IsSame(b))
        {
            return true;
        }
        return false;
    }
    public bool IsSame(Edge other)
    {
        if (other.GetA().IsSame(a) && other.GetB().IsSame(b))
        {
            return true;  
        }
        return false;
    }


    
}