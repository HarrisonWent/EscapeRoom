using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RainbowTilesButton : MonoBehaviour
{
    private RainbowTiles RT;
    public int ButtonID, CurrentColour;
    public Material[] Colours;

    private void Start()
    {
        RT = transform.GetComponentInParent<RainbowTiles>();
        if (gameObject.tag != "Check") 
        {
            CurrentColour = Random.Range(0, 4);
            ChangeColour();
        }
    }

    private void OnMouseOver() 
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            if (gameObject.tag == "Check")
            {
                RT.CurrentAttemptString = "";
                RT.CheckAttempt();
                return;
            }

            CurrentColour++;

            if (CurrentColour > 3)
            {
                CurrentColour = 0;
            }            

            ChangeColour();
        }
    }

    void ChangeColour()
    {
        RT.CurrentAttempt[ButtonID] = CurrentColour;

        switch (CurrentColour)
        {
            case 0:
                gameObject.GetComponent<MeshRenderer>().material = Colours[0];
                break;
            case 1:
                gameObject.GetComponent<MeshRenderer>().material = Colours[1];
                break;
            case 2:
                gameObject.GetComponent<MeshRenderer>().material = Colours[2];
                break;
            case 3:
                gameObject.GetComponent<MeshRenderer>().material = Colours[3];
                break;
        }
    }
}
