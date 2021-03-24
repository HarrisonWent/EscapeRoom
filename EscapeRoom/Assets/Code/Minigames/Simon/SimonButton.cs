using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SimonButton : MonoBehaviour
{
    public int ButtonID;
    private SimonSays SimonObject;

    void Start()
    {
        SimonObject = GetComponentInParent<SimonSays>();
    }


    private void OnMouseOver()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            SimonObject.CheckStep(char.Parse(ButtonID.ToString()));
        }
    }
}
