using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPelletGeneration : MonoBehaviour
{
    [SerializeField]
    private GameObject pellets;

    public LayerMask noFoodMask;

    // Start is called before the first frame update
    void Start()
    {
        //Loop to create all the food pellets where no intersection with something marked unwalkable
        for (float i = -12.50f; i <= 12.50f; i++)
        {
            for (float j = -13.50f; j <= 13.50; j++)
            {
                //Check for unwalkable mask
                if (!Physics.CheckSphere(new Vector3(j, 0.5f, i), 0.25f, noFoodMask))
                {
                    PhotonNetwork.InstantiateRoomObject("Food Pellet", new Vector3(j, 0.5f, i), Quaternion.identity);
                }
            }
        }
    }

}
