using System.Collections.Generic;
using UnityEngine;
public class MobileAgent
{
    private GameObject mobileAgent;
    private Vector3 positionStart;
    private GameObject goal;
    private MobileAgentInfo info;
    public MobileAgent(MobileAgentInfo pInfo, GameObject pMobileAgent, GameObject pGoal)
    {
        info = pInfo;
        mobileAgent = pMobileAgent;
        goal = pGoal;
        positionStart = mobileAgent.transform.position;
    }
    public void ResetStart()
    {
        mobileAgent.transform.GetChild(0).position = positionStart;
    }
    public GameObject GetGoal()
    {
        return goal;
    }
    public GameObject GetObj()
    {
        return mobileAgent;
    }
    public MobileAgentInfo GetInfo()
    {
        return info;
    }
    public void Move(List<Vertex> path)
    {
        mobileAgent.GetComponentInChildren<MobileAgentMvt>().MoveAlongPath(path);
    }

    // public void SpawnGoal(MobileAgentInfo info, Vector3 pos)
    // {
    //     Quaternion rot = Quaternion.Euler(90f, 0, 0);
    //     Vector3 newPos = new Vector3(pos.x, pos.y + 1f, pos.z);
    //     goal = Instantiate(goal_prefab, newPos, rot);
    //     goal.transform.localScale = new Vector3(info.size, info.size, info.size);
    // }
}