using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PuzzleManager : MonoBehaviour
{
    public static float TotalTime;
    private int QuizzedPlayerNumber = 0;
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
            if (next) 
            {
                Debug.Log("Set quizzed player to next");
                QuizzedPlayerNumber = p.ActorNumber;
                set = true; 
                break; 
            }
            if(p.ActorNumber == QuizzedPlayerNumber)
            {
                next = true;
            }
        }
        if (!set) 
        {
            Debug.Log("Not set");
            QuizzedPlayerNumber = PhotonNetwork.PlayerList[0].ActorNumber; 
        }

        GetComponent<PhotonView>().RPC("StartPuzzle", RpcTarget.AllViaServer,Puzzles[Random.Range(0,Puzzles.Length)].PuzzleName, QuizzedPlayerNumber);
    }

    [PunRPC]
    public void StartPuzzle(string PuzzleName,int PlayerActorNumber)
    {
        QuizzedPlayerNumber = PlayerActorNumber;

        Debug.Log("Quizzed player: " + QuizzedPlayerNumber);
        Debug.Log("Client start puzzle: " + PuzzleName);
        Debug.Log("Client actor number: " + PhotonNetwork.LocalPlayer.ActorNumber);

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
        if(QuizzedPlayerNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.Log("Quizzed player");
            Camera.main.transform.position = Hotseat.position;
            Camera.main.transform.rotation = Hotseat.rotation;
            SelectedPuzzle.StartPuzzleAsPlayer.Invoke();
        }
        else
        {
            Debug.Log("Helping player");
            Camera.main.transform.position = HintSeat.position;
            Camera.main.transform.rotation = HintSeat.rotation;
            SelectedPuzzle.StartPuzzleAsHelper.Invoke();
        }
    }
}
