#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class DebugCircle : MonoBehaviour
{
    public float radius = 1f;
    public Color color = Color.green;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = color;
        Handles.DrawWireDisc(transform.position, Vector3.up, radius);
    }
#endif
}