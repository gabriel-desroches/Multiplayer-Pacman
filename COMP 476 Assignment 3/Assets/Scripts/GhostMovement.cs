using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Will implement all ghost movements here, use enums to differentiate them.
public class GhostMovement : MonoBehaviour
{
    private enum GhostTypes { Blinky, Pinky, Inky, Clyde}; 
    private NavMeshAgent _NavMeshAgent;

    [SerializeField]
    private GhostTypes ghostType;
    [SerializeField]
    private Transform pacmanTranform;

    private float speed = 0.1f;
    private Vector3 dest;
    

    // Start is called before the first frame update
    void Start()
    {
        _NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    
    void FixedUpdate()
    {
        dest = pacmanTranform.position;

        if (ghostType == GhostTypes.Blinky)
        {
            _NavMeshAgent.SetDestination(dest);
        }

    }

    private void SetDestination()
    {
        
    }

}
