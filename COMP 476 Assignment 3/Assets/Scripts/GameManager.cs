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

    public Canvas scoreUI;
    public Canvas endOfGameUI;
    public Text[] playerNameUI;
    public Text[] playerScoreUI;
    public Text winnerText;
    public Text winnerList;

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

        scoreUI.gameObject.SetActive(false);
        endOfGameUI.gameObject.SetActive(true);

        List<string> winnerNames = FindWinners();

        if (winnerNames.Count == 1) winnerText.text = "Winner:";
        else winnerText.text = "Winners:";

        foreach (string name in winnerNames)
        {
            winnerList.text += name + "\n";
        }

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

    public List<string> FindWinners()
    {
        int max = -1;
        List<string> names = new List<string>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int playerScore = (int)player.CustomProperties["Score"];
            //Need to handle ties, since number of pellets is divisble by 2, 3, and 4
            if (playerScore == max) names.Add(player.NickName);
            else if (playerScore > max )
            {
                names.Clear();
                names.Add(player.NickName);
                max = playerScore;
            }
        }

        return names;
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " Has Entered the Room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " Has Left the Room");
    }
}
