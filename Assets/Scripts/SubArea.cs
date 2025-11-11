using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubArea : MonoBehaviour
{
    private float speed;
    [SerializeField] private float cost;
    [SerializeField] private List<Transform> verticesT = new List<Transform>();
    [SerializeField] private TextMeshPro costUI;
    public void SetCost(float pCost)
    {
        cost = pCost;
        costUI.text = cost.ToString("F2");
    }
    public float GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(float pSpeed)
    {
        speed = pSpeed;
    }
    public List<Vector2> GetVerticesXZ()
    {
        List<Vector2> verticesXZ = new List<Vector2>();
        foreach(Transform vertexT in verticesT)
        {
            Vector2 posXZ = new Vector2(vertexT.transform.position.x, vertexT.transform.position.z);
            verticesXZ.Add(posXZ);
        }
        return verticesXZ;
    }
    public float GetCost()
    {
        return cost;
    }
    
}
