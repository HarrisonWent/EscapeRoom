using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class SimonSays : MonoBehaviour
{    
    public string CurrentSolution, CurrentAttempt;

    public GameObject SolutionObject;

    public Material[] Colours;

    private bool Cycle = false;

    public UnityEvent EndPuzzleEvent;
    public AudioSource ClickSound;

    void Start()
    {
        //host generate sequence and send to everyone
        if (PhotonNetwork.IsMasterClient)
        {
            string newSolution = "";

            int solutionLength = 4;
            for(int i =0; i<solutionLength;i++)
            {
                newSolution += Random.Range(0, 4).ToString();
            }

            PhotonView.Get(this).RPC("SetupSimonSays", RpcTarget.AllViaServer, newSolution);
        }
    }

    //Set the same solution for all players
    [PunRPC]
    public void SetupSimonSays(string newSolution)
    {
        Debug.Log("Recieved new solution: " + newSolution);
        CurrentSolution = newSolution;
        Cycle = true;
    }


    void Update()
    {
        //Loops the clue sequence
        if(Cycle)
            StartCoroutine(ShowSolution());
    }


    //Checks if the sequence has been complete, call from colour buttons
    public void CheckStep(char NewChar) 
    {
        ClickSound.Play();
        Debug.Log("Add: " + NewChar);
        CurrentAttempt += NewChar;

        //if not equal to the solution clear the attempt
        if(CurrentAttempt.Substring(CurrentAttempt.Length-1,1) != CurrentSolution.Substring(CurrentAttempt.Length-1,1))
        {
            Debug.Log("Sequence incorrect, cleared");
            Debug.Log("You entered: " + CurrentAttempt.Substring(CurrentAttempt.Length - 1, 1) + ", the correct char is: " + CurrentSolution.Substring(CurrentAttempt.Length - 1, 1));

            CurrentAttempt = "";

            return;
        }

        Debug.Log("Step complete");

        //the sequence has been copied correctly, end the puzzle
        if(CurrentAttempt.Length == CurrentSolution.Length)
        {
            Debug.Log("Sequence complete");
            PhotonView.Get(this).RPC("CompletePuzzleAllClients_SimonSays", RpcTarget.AllViaServer);
        }
    }

    //Displays the sequence on the solution clue
    IEnumerator ShowSolution() 
    {
        Cycle = false;
        SolutionObject.gameObject.SetActive(true);
        foreach (char c in CurrentSolution) 
        {
            switch (c) 
            {
                case '0':
                    SolutionObject.gameObject.GetComponent<MeshRenderer>().material = Colours[0];
                    break;
                case '1':
                    SolutionObject.gameObject.GetComponent<MeshRenderer>().material = Colours[1];
                    break;
                case '2':
                    SolutionObject.gameObject.GetComponent<MeshRenderer>().material = Colours[2];
                    break;
                case '3':
                    SolutionObject.gameObject.GetComponent<MeshRenderer>().material = Colours[3];
                    break;
            }
            yield return new WaitForSeconds(1);
            SolutionObject.gameObject.GetComponent<MeshRenderer>().material = Colours[4];
            yield return new WaitForSeconds(0.25f);
        }

        SolutionObject.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        Cycle = true;
    }


    [PunRPC]
    public void CompletePuzzleAllClients_SimonSays()
    {        
        Invoke("PassTheBox", 1);
    }

    private void PassTheBox()
    {
        EndPuzzleEvent.Invoke();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Pass the box!");
            FindObjectOfType<PuzzleManager>().HostPassTheBox();
        }
    }
}
