using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.UI;

public class RainbowTiles : MonoBehaviour
{
    public int[] CurrentAttempt;
    public string CurrentSolution;
    public string CurrentAttemptString;

    public GameObject CompleteObject;
    public Material Green;

    public UnityEvent EndPuzzleEvent;

    public Material[] Colours;
    public MeshRenderer[] RainbowClues;

    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            HostSetup();
        }
    }

    /// <summary>
    /// Host generates random solution and sends it to the clients
    /// </summary>
    public void HostSetup()
    {
        string solution = "";
        for (int i = 0; i < 12; i++)
        {
            int RandomID = Random.Range(0, 4);
            //Debug.Log(RandomID);
            solution += RandomID.ToString();
        }

        PhotonView.Get(this).RPC("SetupRainbowTiles", RpcTarget.AllViaServer, solution);
    }

    /// <summary>
    /// Received from host and Sets the same solution for all players
    /// </summary>
    /// <param name="Solution"></param>
    [PunRPC]
    public void SetupRainbowTiles(string Solution)
    {
        CurrentSolution = Solution;

        //setup clue screen for em
        int pos = 0;

        foreach (MeshRenderer MR in RainbowClues)
        {
            Material m = Colours[int.Parse(CurrentSolution.Substring(pos, 1))];            

            MR.material = m;
            pos++;
        }
    }

    /// <summary>
    /// Checks if the current sequence is equal to the answer, finishes the puzzle if so
    /// </summary>
    public void CheckAttempt() 
    {
        Debug.Log("Check attempt");
        for (int i = 0; i < CurrentAttempt.Length; i++) 
        {
            Debug.Log("Incorrect");
            CurrentAttemptString += CurrentAttempt[i].ToString();
        }

        if (CurrentAttemptString == CurrentSolution) 
        {
            Debug.Log("Correct");
            PhotonView.Get(this).RPC("CompletePuzzleAllClients_RainbowTiles", RpcTarget.AllViaServer);
        }
    }

    /// <summary>
    /// Completes the puzzle, calling the pass box
    /// </summary>
    [PunRPC]
    public void CompletePuzzleAllClients_RainbowTiles()
    {
        CompleteObject.GetComponent<MeshRenderer>().material = Green;
        Invoke("PassTheBox_Rainbow", 1);
    }

    /// <summary>
    /// Tells the host to pass the box to the next player
    /// </summary>
    private void PassTheBox_Rainbow()
    {
        EndPuzzleEvent.Invoke();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Pass the box!");
            FindObjectOfType<PuzzleManager>().HostPassTheBox();
        }
    }
}
