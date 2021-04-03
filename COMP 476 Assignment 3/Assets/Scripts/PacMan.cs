using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Movement with help from https://noobtuts.com/unity/2d-pacman-game
public class PacMan : MonoBehaviour
{
    
    public LayerMask unwalkableLayer;
    Vector3 dest = Vector3.zero;

    private float speed = 0.1f;
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        dest = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleInputs();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pellet"))
        {
            Destroy(other.gameObject);
            score++;
        }
        else if (other.CompareTag("Super Pellet"))
        {
            Destroy(other.gameObject);
            speed = 0.15f;
            StartCoroutine(ResetSpeed());
        }    
    }

    void HandleInputs()
    {
        // Move towards destination
        Vector3 p = Vector3.MoveTowards(transform.position, dest, speed);
        GetComponent<Rigidbody>().MovePosition(p);

        // Check for Input if not moving
        if ((Vector3)transform.position == dest)
        {
            //Switch this to use axes
            if (Input.GetKey(KeyCode.W) && valid(Vector3.forward))
                dest = (Vector3)transform.position + Vector3.forward;
            if (Input.GetKey(KeyCode.D) && valid(Vector3.right))
                dest = (Vector3)transform.position + Vector3.right;
            if (Input.GetKey(KeyCode.S) && valid(-Vector3.forward))
                dest = (Vector3)transform.position - Vector3.forward;
            if (Input.GetKey(KeyCode.A) && valid(-Vector3.right))
                dest = (Vector3)transform.position - Vector3.right;
        }
    }

    bool valid(Vector3 dir)
    {
        // Cast Line from 'next to Pac-Man' to 'Pac-Man'
        Vector3 pos = transform.position;
        Vector3 target = pos + dir;
       
        return !Physics.Linecast(pos, pos + dir, unwalkableLayer);
    }

    IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(3.0f);
        speed = 0.1f;
    }
}
