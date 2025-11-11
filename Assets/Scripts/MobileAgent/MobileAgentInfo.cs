using UnityEngine;

[CreateAssetMenu(fileName = "MobileAgentInfo", menuName = "Scriptable Objects/MobileAgentInfo")]
public class MobileAgentInfo : ScriptableObject
{
    public MobileAgentType type;
    public GameObject obj;
    public GameObject goal_pref;
    public float size;
}
