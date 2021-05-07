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

    public Transform Hotseat, HintSeat;
    public GameObject PuzzleBox;
    public Transform PuzzleBoxSpawnTransform;
    private GameObject SpawnedBox;
    public Text HelpText;

    public LayerMask PuzzleMask;
    public LayerMask ClueMask;
    public int PuzzlesComplete = 0, PuzzlesToComplete = 5;//puzzles to complete is 1 more than the actual amount of puzzles as it is called at the start of the game

    //Host only
    /// <summary>
    /// Called to the host, passes the box to the next player
    /// </summary>
    public void HostPassTheBox()
    {        
        if(!SpawnedBox)
        {
            SpawnedBox = PhotonNetwork.Instantiate(PuzzleBox.name, PuzzleBoxSpawnTransform.position, PuzzleBoxSpawnTransform.rotation);
        }

        //Check for game over
        PuzzlesComplete++;
        if (PuzzlesComplete == PuzzlesToComplete)
        {
            GetComponent<PhotonView>().RPC("GameOver", RpcTarget.AllViaServer);
            return;
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

        GetComponent<PhotonView>().RPC("ClientPassTheBox", RpcTarget.AllViaServer, QuizzedPlayerNumber);
    }

    /// <summary>
    /// Called on all players machines updates who has the box
    /// </summary>
    /// <param name="PlayerActorNumber">Actor number of player who is now answering a puzzle</param>
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
            FindObjectOfType<BoxModeSwitch>().SwapBox(true);
            Camera.main.cullingMask = PuzzleMask;
            HelpText.text = "Use WASD keys to rotate. Press 'R' to roll the box.";
        }
        else
        {
            Debug.Log("Helping player");
            FindObjectOfType<BoxModeSwitch>().SwapBox(false);
            Camera.main.cullingMask = ClueMask;
            HelpText.text = "You are helping the player. They control the box!";
        }
    }

    public Animation GameOverAnimation,CameraAnimation;
    /// <summary>
    /// Called when all puzzles complete
    /// </summary>
    [PunRPC]
    public void GameOver()
    {
        Debug.LogWarning("Congrats the game has been won");

        HelpText.text = "";

        GameOverAnimation.Play();
        CameraAnimation.Play();
        FindObjectOfType<NetworkGameManager>().InfoText.text = "You have solved the puzzles and escaped the island!";
        Invoke("LeaveMatch", GameOverAnimation.clip.length);
    }

    void LeaveMatch()
    {
        PhotonNetwork.LeaveRoom();
    }
}
