using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        print("Room has been joined.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Create a room
        PhotonNetwork.CreateRoom("");
    }

    public override void OnCreatedRoom()
    {
        print("Room has been created.")
    }
}
