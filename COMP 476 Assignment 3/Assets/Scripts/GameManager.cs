using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    AudioClip[] audioClips;
    AudioSource audioSource;
    public GameObject playerPrefab;
    public Text[] playerNameUI;
    public Text[] playerScoreUI;

    private GameObject[] pellets;
    private GameObject[] superPellets;

    public static GameManager Instance;
    public static int numOfPellets = -1;

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

        StartCoroutine(findPelletsAfterDelay());
      
    }

    public void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (numOfPellets == 0)
            {
                endGame();
            }
        }

        print(numOfPellets);
        
        for (int i = 0; i < 4; i++)
        {
            try
            {
                playerNameUI[i].text = PhotonNetwork.PlayerList[i].NickName;
                playerScoreUI[i].text = PhotonNetwork.PlayerList[i].CustomProperties["Score"].ToString();
            }
            catch (Exception e) //For Empty slots
            {
                playerNameUI[i].text = "";
                playerScoreUI[i].text = "";
            }
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

    IEnumerator findPelletsAfterDelay()
    {
        //Delay to make sure pellets are spawned on scene in time
        yield return new WaitForSeconds(1.0f);
        //Soft coded to work with any level and detect how many pellets. Used for end of game.
        pellets = GameObject.FindGameObjectsWithTag("Pellet");
        superPellets = GameObject.FindGameObjectsWithTag("Super Pellet");
        numOfPellets = pellets.Length + superPellets.Length;

        //Clear for memory purposes
        pellets = null;
        superPellets = null;
    }

    //Handle the Score UI
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " Has Entered the Room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " Has Left the Room");
    }
}
