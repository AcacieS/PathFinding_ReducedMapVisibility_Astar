using UnityEngine;

public class VertexProp: MonoBehaviour
{
    public VertexType type;
    private Vertex vertex;
    private float cost;

    public void SetVertex(Vertex pVertex)
    {
        vertex = pVertex;
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.tag == "SubArea")
    //     {
    //         Debug.Log("Detect subArea");
    //         cost = other.gameObject.GetComponent<SubArea>().GetCost();
    //         vertex.SetCost(cost);
    //     }
    // }


}