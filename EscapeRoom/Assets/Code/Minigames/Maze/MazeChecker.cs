using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MazeChecker : MonoBehaviour
{
    public GameObject CompleteLight;
    public Material Green;
    bool complete = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MazePlayer" && !complete) 
        {
            complete = true;

            CompleteLight.GetComponent<Renderer>().material = Green;

            if (PhotonNetwork.LocalPlayer.ActorNumber == PuzzleManager.QuizzedPlayerNumber)
            {
                GetComponentInParent<MazeManager>().PlayerCompletePuzzle();
            }            
        }

    }
}
