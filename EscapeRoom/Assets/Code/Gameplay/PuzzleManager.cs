using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PuzzleManager : MonoBehaviour
{
    public static float TotalTime;
    private string QuizzedPlayerID = "";
    public PlayablePuzzle[] Puzzles;
    public int PuzzlesToWin = 2;
    public Transform Hotseat, HintSeat;

    [System.Serializable]
    public struct PlayablePuzzle
    {
        public string PuzzleName;
        public UnityEvent StartPuzzleAsPlayer;
        public UnityEvent StartPuzzleAsHelper;
    }

    //Host only
    public void HostStartNextPuzzle()
    {
        //Change to the next player ID after the current one
        bool next = false, set = false;
        foreach(Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (next) { QuizzedPlayerID = p.UserId;set = true; break; }
            if(p.UserId == QuizzedPlayerID)
            {
                next = true;
            }
        }
        if (!set) { QuizzedPlayerID = PhotonNetwork.PlayerList[0].UserId; }

        GetComponent<PhotonView>().RPC("StartPuzzle", RpcTarget.AllViaServer,Puzzles[Random.Range(0,Puzzles.Length)].PuzzleName,QuizzedPlayerID);
    }

    [PunRPC]
    public void StartPuzzle(string PuzzleName,string PlayerID)
    {
        QuizzedPlayerID = PlayerID;

        Debug.Log("Client start puzzle: " + PuzzleName);
        Debug.Log("Client ID: " + PhotonNetwork.LocalPlayer.UserId);

        //Start the selected puzzle
        PlayablePuzzle SelectedPuzzle = new PlayablePuzzle();
        foreach(PlayablePuzzle PP in Puzzles)
        {
            if(PP.PuzzleName == PuzzleName)
            {
                SelectedPuzzle = PP;
                break;
            }
        }

        //Switch the cameras depending on whos in the hotseat
        if(QuizzedPlayerID == PhotonNetwork.LocalPlayer.UserId)
        {
            Camera.main.transform.position = Hotseat.position;
            Camera.main.transform.rotation = Hotseat.rotation;
            SelectedPuzzle.StartPuzzleAsPlayer.Invoke();
        }
        else
        {
            Camera.main.transform.position = HintSeat.position;
            Camera.main.transform.rotation = HintSeat.rotation;
            SelectedPuzzle.StartPuzzleAsHelper.Invoke();
        }
    }
}
