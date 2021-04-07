using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Will implement all ghost movements here, use enums to differentiate them.
public class GhostMovement : MonoBehaviour
{
    private enum GhostTypes { Blinky, Pinky, Inky, Clyde}; 

    [SerializeField]
    private GhostTypes ghostType;
    [SerializeField]
    private Transform pacmanTranform;

    private float speed = 0.1f;
    private Vector3 dest;

    Vector3[] path;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool canAskForPath = true;
    void FixedUpdate()
    {
        //dest = pacmanTranform.position;

        if (ghostType == GhostTypes.Blinky && canAskForPath)
        {
            canAskForPath = false;
            //StartCoroutine(determinePath());
        }

    }

    IEnumerator determinePath()
    {
        PathRequestManager.RequestPath(transform.position, dest, onPathFound);
        yield return new WaitForSeconds(0.1f);
        canAskForPath = true;
    }

    public void onPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            if (path.Length != 0)
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
    }

    private int targetIndex;
    IEnumerator FollowPath()
    {

        Vector3 currentWaypoint = path[0]; //fix bug here
        targetIndex = 0;
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    targetIndex = 0;
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Vector3 p = Vector3.MoveTowards(transform.position, currentWaypoint, speed);
            GetComponent<Rigidbody>().MovePosition(p);
            yield return null;
        }
    }

}
