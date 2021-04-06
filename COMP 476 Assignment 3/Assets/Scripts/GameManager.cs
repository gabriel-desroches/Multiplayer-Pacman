using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    AudioClip[] audioClips;
    AudioSource audioSource;
    public GameObject playerPrefab;

    private GameObject[] pellets;
    private GameObject[] superPellets;

    public static GameManager Instance;
    public static int numOfPellets;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Start()
    {
        Instance = this;

        if (playerPrefab == null)
        {
            Debug.LogError("Missing playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            if (PacMan.LocalPlayerInstance == null)
            {
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0.5f, 0.75f, -3.5f), Quaternion.identity, 0);
            }
        }



        //Soft coded to work with any level and detect how many pellets. Used for end of game.
        pellets = GameObject.FindGameObjectsWithTag("Pellet");
        superPellets = GameObject.FindGameObjectsWithTag("Super Pellet");
        numOfPellets = pellets.Length + superPellets.Length;

        //Clear for memory purposes
        pellets = null;
        superPellets = null;
    }

    public void Update()
    {
        if(numOfPellets == 0)
        {
            endGame();
        }

        if (!audioSource.isPlaying)
        {
            audioSource.clip = audioClips[0];
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void endGame()
    {
        Time.timeScale = 0;

    }

}
