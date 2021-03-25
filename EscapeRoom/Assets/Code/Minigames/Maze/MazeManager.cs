using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class MazeManager : MonoBehaviour
{
    public UnityEvent EndPuzzleEvent;

    public void PlayerCompletePuzzle()
    {
        Debug.Log("Complete maze");
        PhotonView.Get(this).RPC("CompletePuzzleAllClients_Maze", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void CompletePuzzleAllClients_Maze()
    {
        Invoke("PassTheBox_Maze", 1);
    }

    private void PassTheBox_Maze()
    {
        EndPuzzleEvent.Invoke();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Pass the box!");
            FindObjectOfType<PuzzleManager>().HostPassTheBox();
        }
    }
}
