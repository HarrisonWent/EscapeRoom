using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static float TotalTime;
    public static int QuizzedPlayerNumber = 0;
    //public PlayablePuzzle[] Puzzles;
    //public int PuzzlesToWin = 2;
    public Transform Hotseat, HintSeat;
    public GameObject PuzzleBox;
    public Transform PuzzleBoxSpawnTransform;
    private GameObject SpawnedBox;
    public Text HelpText;
    //[System.Serializable]
    //public struct PlayablePuzzle
    //{
    //    public string PuzzleName;
    //    public UnityEvent StartPuzzleAsPlayer;
    //    public UnityEvent StartPuzzleAsHelper;
    //}

    public LayerMask PuzzleMask;
    public LayerMask ClueMask;

    //Host only
    public void HostPassTheBox()//todo call this when the box is rotated and detect which puzzle is being viewed
    {        
        if(!SpawnedBox)
        {
            SpawnedBox = PhotonNetwork.Instantiate(PuzzleBox.name, PuzzleBoxSpawnTransform.position, PuzzleBoxSpawnTransform.rotation);
        }

        //Change to the next player ID after the current one
        bool next = false, set = false;
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (next)
            {
                Debug.Log("Set quizzed player to next");
                QuizzedPlayerNumber = p.ActorNumber;
                set = true;
                break;
            }
            if (p.ActorNumber == QuizzedPlayerNumber)
            {
                next = true;
            }
        }
        if (!set)
        {
            Debug.Log("Not set");
            QuizzedPlayerNumber = PhotonNetwork.PlayerList[0].ActorNumber;
        }

        //GetComponent<PhotonView>().TransferOwnership()
        //PlayablePuzzle CurrentSelectedPuzzle = null;

        GetComponent<PhotonView>().RPC("ClientPassTheBox", RpcTarget.AllViaServer, QuizzedPlayerNumber);
    }

    [PunRPC]
    public void ClientPassTheBox(int PlayerActorNumber)
    {
        QuizzedPlayerNumber = PlayerActorNumber;

        Debug.Log("Quizzed player: " + QuizzedPlayerNumber);
       // Debug.Log("Client start puzzle: " + PuzzleName);
        Debug.Log("Client actor number: " + PhotonNetwork.LocalPlayer.ActorNumber);

        //Move ownership of the box transform to the new player so they can control it
        if(PhotonNetwork.IsMasterClient)
        {
            Photon.Realtime.Player p = null;
            foreach(Photon.Realtime.Player pp in PhotonNetwork.PlayerList)
            {
                if(pp.ActorNumber == QuizzedPlayerNumber)
                {
                    p = pp;
                    break;
                }
            }

            SpawnedBox.GetComponent<PhotonView>().TransferOwnership(p);
        }

        //if player view normal else view help screen here
        if (QuizzedPlayerNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.Log("Quizzed player");
            Camera.main.cullingMask = PuzzleMask;
            HelpText.text = "Use WASD keys to rotate. Press 'R' to roll the box.";
        }
        else
        {
            Debug.Log("Helping player");
            Camera.main.cullingMask = ClueMask;
            HelpText.text = "You are helping the player. They control the box!";
        }
    }
}
