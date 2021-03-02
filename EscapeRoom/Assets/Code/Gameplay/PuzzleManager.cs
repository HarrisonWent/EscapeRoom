using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PuzzleManager : MonoBehaviour
{
    public static float TotalTime;
    private int QuizzedPlayer =0;
    public PlayablePuzzle[] Puzzles;
    public int PuzzlesToWin = 2;
    public Transform Hotseat, HintSeat;

    [System.Serializable]
    public struct PlayablePuzzle
    {
        public string PuzzleName;
        public UnityEvent PuzzleEvent;
    }

    public void HostStartNextPuzzle()
    {
        QuizzedPlayer++;
        if (QuizzedPlayer == PhotonNetwork.PlayerList.Length) { QuizzedPlayer = 0; }
        
        GetComponent<PhotonView>().RPC("StartPuzzle", RpcTarget.AllViaServer,Puzzles[Random.Range(0,Puzzles.Length)].PuzzleName,QuizzedPlayer);
    }

    [PunRPC]
    public void StartPuzzle(string PuzzleName,int PlayerInHotseat)
    {
        QuizzedPlayer = PlayerInHotseat;

        Debug.Log("Client start puzzle: " + PuzzleName);
        Debug.Log("Client ID: " + PhotonNetwork.LocalPlayer.UserId);

        //Start the selected puzzle
        foreach(PlayablePuzzle PP in Puzzles)
        {
            if(PP.PuzzleName == PuzzleName)
            {
                PP.PuzzleEvent.Invoke();
                break;
            }
        }

        //Switch the cameras depending on whos in the hotseat
        if(PhotonNetwork.PlayerList[QuizzedPlayer] == PhotonNetwork.LocalPlayer)
        {
            Camera.main.transform.position = Hotseat.position;
            Camera.main.transform.rotation = Hotseat.rotation;
        }
        else
        {
            Camera.main.transform.position = HintSeat.position;
            Camera.main.transform.rotation = HintSeat.rotation;
        }
    }
}
