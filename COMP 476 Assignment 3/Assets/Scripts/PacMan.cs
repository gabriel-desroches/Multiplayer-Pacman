using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Movement with help from https://noobtuts.com/unity/2d-pacman-game
public class PacMan : MonoBehaviourPunCallbacks
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    public LayerMask unwalkableLayer;

    [SerializeField]
    private AudioClip[] audioClips;
    private AudioSource audioSource;
    private float speed = 0.1f;
    private int score = 0;
    private Vector3 dest = Vector3.zero;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();    
    }

    void Start()
    {
        dest = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            HandleInputs();
        }
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.CompareTag("Pellet"))
        {
            Destroy(other.gameObject);
            score++;
            GameManager.numOfPellets--; //Encapsulation todo
            StartCoroutine(playEatingSound());
        }
        else if (other.CompareTag("Super Pellet"))
        {
            Destroy(other.gameObject);
            speed = 0.15f;
            GameManager.numOfPellets--;
            StartCoroutine(ResetSpeed());
            StartCoroutine(playEatingSound());
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

    IEnumerator playEatingSound()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
        yield return new WaitForSeconds(audioClips[0].length + 0.010f);
        audioSource.clip = audioClips[1];
        audioSource.Play();
    }

}
