using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Will implement all ghost movements here, use enums to differentiate them.
public class GhostMovement : MonoBehaviourPunCallbacks
{
    private enum GhostTypes { Blinky, Pinky, Inky, Clyde}; 

    [SerializeField]
    private GhostTypes ghostType;
    [SerializeField]
    private Transform pacmanTranform;

    //Ghost will be slightly slower than players since they have 0 delay around
    //corners and wonky connections means they can sometimes be faster, sometimes slower.
    private float speed = 0.085f;
    private Vector3 dest;

    Vector3[] path;

    void Start()
    {
        StartCoroutine(delayStart());
    }

    //Each player has exactly one ghost assigned to them. Ghost will activate and deactive
    //Depending on player count. TODO: Reset appropriate ghost on player leave room
    bool canAskForPath = false;
    void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (ghostType == GhostTypes.Blinky && canAskForPath && PhotonNetwork.PlayerList.Length >= 1)
            {
                canAskForPath = false;
                dest = (Vector3)PhotonNetwork.PlayerList[0].CustomProperties["Location"];
                StartCoroutine(determinePath());
            }

            if (ghostType == GhostTypes.Inky && canAskForPath && PhotonNetwork.PlayerList.Length >= 2)
            {
                canAskForPath = false;
                dest = (Vector3)PhotonNetwork.PlayerList[1].CustomProperties["Location"];
                StartCoroutine(determinePath());
            }

            if (ghostType == GhostTypes.Pinky && canAskForPath && PhotonNetwork.PlayerList.Length >= 3)
            {
                canAskForPath = false;
                dest = (Vector3)PhotonNetwork.PlayerList[2].CustomProperties["Location"];
                StartCoroutine(determinePath());
            }

            if (ghostType == GhostTypes.Clyde && canAskForPath && PhotonNetwork.PlayerList.Length >= 4)
            {
                canAskForPath = false;
                dest = (Vector3)PhotonNetwork.PlayerList[3].CustomProperties["Location"];
                StartCoroutine(determinePath());
            }
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

    IEnumerator delayStart()
    {
        yield return new WaitForSeconds(2.0f);
        canAskForPath = true;
    }

    //Delay the pathfinding to give time for the custom properties to be set, results in a 
    //NullReferenceException otherwise
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        canAskForPath = false;
        StartCoroutine(delayStart());
    }

}
