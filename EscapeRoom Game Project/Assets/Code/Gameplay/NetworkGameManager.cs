using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public class NetworkGameManager : MonoBehaviourPunCallbacks
{
    public static NetworkGameManager Instance = null;

    public Text InfoText;

    #region UNITY

    public void Awake()
    {
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
    }

    public void Start()
    {
        Hashtable props = new Hashtable
        {
            {PuzzleGame.PLAYER_LOADED_LEVEL, true}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
    }

    #endregion

    #region COROUTINES

    /// <summary>
    /// Called at the end of the game diplaying the ending text, returns to menu
    /// </summary>
    /// <param name="winner"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    private IEnumerator EndOfGame(string winner, int score)
    {
        float timer = 5.0f;

        while (timer > 0.0f)
        {
            InfoText.text = string.Format("You have won!\n\n\nReturning to login screen in {2} seconds.", winner, score, timer.ToString("n2"));

            yield return new WaitForEndOfFrame();

            timer -= Time.deltaTime;
        }

        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            //if is master client (this one)
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //CheckEndOfGame(); todo return to menu if player left or pass the box if they were playing
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
        int startTimestamp;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

        if (changedProps.ContainsKey(PuzzleGame.PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                if (!startTimeIsSet)
                {
                    CountdownTimer.SetStartTime();
                }
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ", this.InfoText);
                InfoText.text = "Waiting for other players...";
            }
        }
    }

    #endregion

    public Animation startAnimation;
    public GameObject HelpBoard, HelpCamera;
    /// <summary>
    /// called by OnCountdownTimerIsExpired() when the timer ended, starts the first puzzle
    /// </summary>
    private IEnumerator StartGame()
    {
        Debug.Log("StartGame!");
        startAnimation.Play("start");
        InfoText.text = string.Format("Solve the puzzles to raise the bridge and escape the island!");


        while (startAnimation.isPlaying)
        {           
            yield return new WaitForEndOfFrame();
        }

        InfoText.text = string.Format("");

        //PhotonNetwork.CurrentRoom.PlayerCount) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
        // spawn players here PhotonNetwork.Instantiate("Spaceship", position, rotation, 0);      // avoid this call on rejoin (ship was network instantiated before)

        //If singleplayer spawn some help
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            HelpBoard.SetActive(true);
            HelpCamera.SetActive(true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Start as host");
            FindObjectOfType<PuzzleManager>().HostPassTheBox();
        }
        else
        {
            Debug.Log("Not the host");
        }
    }

    /// <summary>
    /// Checks if all players are in the game level
    /// </summary>
    /// <returns>If all loaded</returns>
    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(PuzzleGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

    private void OnCountdownTimerIsExpired()
    {
        StartCoroutine(StartGame());
    }
}

