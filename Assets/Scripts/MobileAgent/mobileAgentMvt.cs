using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MobileAgentMvt: MonoBehaviour
{
    public float moveSpeed = 1f;

    private Coroutine moveCoroutine;

    public void MoveAlongPath(List<Vertex> path)
    {
        Debug.LogWarning("Should Move now");
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(FollowPath(path));
    }

    private IEnumerator FollowPath(List<Vertex> path)
    {
        foreach (Vertex v in path)
        {
            Vector3 target = v.getPos();
            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                // Optional: rotate character to face target
                // Vector3 direction = (target - transform.position).normalized;
                // if (direction != Vector3.zero)
                //     transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);
                yield return null;
            }
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "SubArea")
        {
            moveSpeed = other.gameObject.GetComponent<SubArea>().GetSpeed();
        }
    }


}