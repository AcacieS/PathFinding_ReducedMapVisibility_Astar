using UnityEngine;

public class CollisionColorChanger : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Color collisionColor = Color.red;
    [SerializeField] private Color defaultColor = Color.white;
    private Renderer[] childRenderers;
    private bool hasPos = false;

    private GameObject ground;
    void Awake()
    {
         // Get all renderers in children (includes nested ones)
        childRenderers = GetComponentsInChildren<Renderer>();
    }
    void Start()
    {
        SetColor(defaultColor);
        ground = GameObject.FindWithTag("Ground");
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!hasPos && (((1 << other.gameObject.layer) & targetLayers) != 0))
        {
            SetColor(collisionColor);
        }
    }
    
    private Vector3 RandomSpawnPos(){
        hasPos = false;
        Debug.LogWarning("Change spawnPos");
        BoxCollider groundCollider = ground.GetComponent<BoxCollider>();
        Vector3 groundSize = Vector3.Scale(groundCollider.size, ground.transform.localScale);
        Vector3 groundCenter = groundCollider.transform.position;

        float randomX = Random.Range(
            groundCenter.x - groundSize.x / 2f,
            groundCenter.x + groundSize.x / 2f
        );
        float randomZ = Random.Range(
            groundCenter.z - groundSize.z / 2f,
            groundCenter.z + groundSize.z / 2f
        );

        Vector3 spawnPos = new Vector3(randomX, groundCenter.y+1f, randomZ);
        return spawnPos;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!hasPos &&(((1 << other.gameObject.layer) & targetLayers) != 0))
        {
            Debug.Log(" has Pos " + gameObject.name);
            hasPos = true;
            SetColor(defaultColor);
        }

    }
    public bool IsGreen()
    {
        if(childRenderers[0].material.color == defaultColor)
        {
            return true;
        }
        return false;
    }
    private void SetColor(Color color)
    {
        childRenderers = System.Array.FindAll(childRenderers, r => r != null);
        foreach (Renderer rend in childRenderers)
        {
            if (rend != null && rend.gameObject.tag != "Vertex")
                rend.material.color = color;
        }
        if(color == collisionColor)
        {
            transform.position = RandomSpawnPos();
        }
    }
}
