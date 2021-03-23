using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectiveMemoryPuzzle : Puzzlebase
{
    [System.Serializable]
    public struct Variant
    {
        public Sprite[] Images;
        public QuestionAnswerSet[] SeqeunceQuestions;        
    }

    [System.Serializable]
    public struct QuestionAnswerSet
    {
        public string Question;
        public string[] AnswerOptions;
        public string CorrectAnswer;
        public string Clue;
    }

    public Variant[] PuzzleVariants;

    public Text QuestionText, AnswerText,ClueQuestiontext,ClueText;
    public Image ImageBoard;
    public Transform AnswerBoard;
    public SelectionOption AnswerButtonPrefab;
    public UnityEvent EndPuzzleEvent;

    Variant SelectedVariant;
    QuestionAnswerSet MySelectedQuestion;

    void Update()
    {
        if (EventSystem.current)
        {
            if (EventSystem.current.currentSelectedGameObject)
            {
                Debug.Log("Over: " + EventSystem.current.currentSelectedGameObject.name);
            }
        }

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    Debug.Log(hit.transform.name);
        //}
    }

    public override void StartPuzzle()//only call for player
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber)
        {
            return;
        }

        Debug.Log("start puzzle");
        int SelectedVairiantNumber = Random.Range(0, PuzzleVariants.Length);
        int Question = Random.Range(0, PuzzleVariants[SelectedVairiantNumber].SeqeunceQuestions.Length);

        PhotonView.Get(this).RPC("InitializePuzzle", RpcTarget.AllViaServer, SelectedVairiantNumber, Question);
    }

    [PunRPC]
    public void InitializePuzzle(int Variant,int Question)//get all clients to run the selected puzzle from the current player
    {
        Debug.Log("Client init puzzle");

        //Same for all players:
        SelectedVariant = PuzzleVariants[Variant];
        //Different question for all players:
        MySelectedQuestion = SelectedVariant.SeqeunceQuestions[Question];

        QuestionText.text = MySelectedQuestion.Question;
        ClueQuestiontext.text = MySelectedQuestion.Question;
        ClueText.text = MySelectedQuestion.Clue;

        //Start sequence
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        Debug.Log("Client run sequence");
        //Remove old quesiton cards
        List<Transform> childs = new List<Transform>();
        childs.AddRange(AnswerBoard.GetComponentsInChildren<Transform>());
        childs.Remove(AnswerBoard);
        while (childs.Count > 0) { Destroy(childs[0].gameObject);childs.RemoveAt(0); }

        AnswerText.enabled = false;

        //Help with reading the question before the sequence
        yield return new WaitForSeconds(1.5f);

        AnswerBoard.gameObject.SetActive(false);
        ImageBoard.enabled = true;

        //Play sequence
        float TimeBetweenImages = 1f;
        WaitForSeconds WSF = new WaitForSeconds(TimeBetweenImages);

        foreach (Sprite s in SelectedVariant.Images)
        {
            ImageBoard.sprite = s;
            yield return WSF;
        }

        ImageBoard.enabled = false;

        //Display answer board
        AnswerBoard.gameObject.SetActive(true);
        foreach (string s in MySelectedQuestion.AnswerOptions)
        {
            SelectionOption newButton = Instantiate(AnswerButtonPrefab, AnswerBoard);
            newButton.Answertext.text = s;
            newButton.MyPuzzle = this;
        }
    }

    public void CheckAnswer(string SelectedAnswer)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber) { return; }

        AnswerBoard.gameObject.SetActive(false);
        AnswerText.text = "";
        AnswerText.enabled = true;

        bool Correct = false;
        if (SelectedAnswer == MySelectedQuestion.CorrectAnswer)
        {
            Correct = true;
            Debug.Log("Correct");
        }
        else
        {
            Debug.Log("Incorrect, you entered: " + SelectedAnswer + ", the answer is: " + MySelectedQuestion.CorrectAnswer);
        }

        GetComponentInParent<PhotonView>().RPC("ShowResult", RpcTarget.AllViaServer,PhotonNetwork.LocalPlayer.NickName,Correct);
    }


    [PunRPC]
    public void ShowResult(string NickName,bool Correct)
    {
        string Result = NickName;
        if (!Correct)
        {
            Result += " answered incorrectly! Restarting the puzzle!";

            //Host restart the puzzle
            if (PhotonNetwork.IsMasterClient)
            {
                Invoke("StartPuzzle", 3);
            }
        }
        else
        {
            Result += " got it correct! Puzzle complete!";

            Invoke("PassTheBox", 3);
        }
        AnswerText.text = Result;
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
