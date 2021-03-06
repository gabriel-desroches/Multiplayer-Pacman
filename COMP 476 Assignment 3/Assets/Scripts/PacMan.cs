using Photon.Pun;

using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    private Vector3 dest = Vector3.zero;
    private Rigidbody rb;

    private void Awake()
    {
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            PacMan.LocalPlayerInstance = this.gameObject;
        }
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        dest = transform.position;

        //Set inital score using custom properties as to sync between all players
        if (photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("Score", 0);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            //This code is janky, used to give the ghosts our location
            //TODO - find better way of doing this
            Hashtable hashTmp = new Hashtable();
            hashTmp.Add("Location", transform.position);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashTmp);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            HandleInputs();
            SendLocation();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //Usually only the master client can Destroy photon network instantiated objects.
        //My fix for this was to request ownership of the photonview's of contacted pellets and to then destroy them
        //However, this fix wasn't working, so I changed the code in the PhotonNetwork API itself to allow any client
        //to destroy network objects.
        if (other.CompareTag("Pellet"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            IncreaseScore();
            photonView.RPC("reduceNumberOfPellets", RpcTarget.All); //Tell all clients to lower the pellet count
            StartCoroutine(playEatingSound());
        }
        else if (other.CompareTag("Super Pellet"))
        {
            PhotonNetwork.Destroy(other.gameObject);
            speed = 0.15f;
            IncreaseScore(); //Uncomment this line and disable pellet generation script on game manager for easy demoing.
            photonView.RPC("reduceNumberOfPellets", RpcTarget.All);
            StartCoroutine(ResetSpeed());
            StartCoroutine(playEatingSound());
        }
        //Teleporters are hardcoded for now, todo: make smoother.
        else if (other.CompareTag("Left Teleporter"))
        {
            //-13.5, 0.75, 4.5 top left
            // -13.5, 0.75, -3.5 bottom left
            Vector3 currentPos = transform.position;
            currentPos.x = 13.5f;
            transform.position = currentPos;
            //Cancel any movement
            dest = currentPos;
            Vector3 p = Vector3.MoveTowards(transform.position, dest, speed);
            rb.MovePosition(p);

        }
        else if (other.CompareTag("Right Teleporter"))
        {
            // 13.5, 0.75, 4.5
            // 13.5, 0.75, -3.5
            Vector3 currentPos = transform.position;
            currentPos.x = -13.5f;
            transform.position = currentPos;
            dest = currentPos;
            Vector3 p = Vector3.MoveTowards(transform.position, dest, speed);
            rb.MovePosition(p);
        }
        else if (other.CompareTag("Ghost"))
        {
            print("saw ghost");
            ResetPosition();
        }
    }

    void HandleInputs()
    {
        // Move towards destination
        Vector3 p = Vector3.MoveTowards(transform.position, dest, speed);
        rb.MovePosition(p);

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

    //Custom properties help from
    //https://forum.photonengine.com/discussion/9937/example-for-custom-properties
    public void IncreaseScore()
    {
        int score = (int)PhotonNetwork.LocalPlayer.CustomProperties["Score"];
        score++;
        Hashtable hash = new Hashtable();
        hash.Add("Score", score);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void SendLocation()
    {
        Hashtable hash = new Hashtable();
        hash.Add("Location", transform.position);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    [PunRPC]
    void reduceNumberOfPellets()
    {
        GameManager.numOfPellets--;
    }


    public void ResetPosition()
    {

        print("reset");
        //Using hardcoded start location for now, need to fix
        transform.position = new Vector3(0.5f, 0.75f, -3.5f);
        dest = transform.position;
        Vector3 p = Vector3.MoveTowards(transform.position, dest, speed);
        rb.MovePosition(p);
        enabled = false;
        enabled = true;


    }

}
