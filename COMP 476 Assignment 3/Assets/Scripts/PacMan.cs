using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Movement with help from https://noobtuts.com/unity/2d-pacman-game
public class PacMan : MonoBehaviour
{
    public float speed = 0.5f;
    public LayerMask unwalkableLayer;
    Vector3 dest = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        dest = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Move towards destination
        Vector3 p = Vector3.MoveTowards(transform.position, dest, speed);
        GetComponent<Rigidbody>().MovePosition(p);

        // Check for Input if not moving
        if ((Vector3)transform.position == dest)
        {

            print(valid(Vector3.forward) + " Up");
            print(valid(Vector3.right) + " Right");
            print(valid(-Vector3.forward) + " down");
            print(valid(-Vector3.right) + " Left");

            if (Input.GetKey(KeyCode.UpArrow) && valid(Vector3.forward))
                dest = (Vector3)transform.position + Vector3.forward;
            if (Input.GetKey(KeyCode.RightArrow) && valid(Vector3.right))
                dest = (Vector3)transform.position + Vector3.right;
            if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector3.forward))
                dest = (Vector3)transform.position - Vector3.forward;
            if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector3.right))
                dest = (Vector3)transform.position - Vector3.right;
        }

        
        //print("Dest " + dest);
        //print("Transform position " + transform.position);
    }

    bool valid(Vector3 dir)
    {
        // Cast Line from 'next to Pac-Man' to 'Pac-Man'
        Vector3 pos = transform.position - new Vector3(0.0f, 0.25f, 0.0f);
        Vector3 target = pos + dir;
       
        return !Physics.Linecast(pos, pos + dir, unwalkableLayer);
    }

}
