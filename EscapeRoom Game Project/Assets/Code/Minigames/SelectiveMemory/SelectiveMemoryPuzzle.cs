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
    public AudioSource ClickButton,MemorySequence;

    Variant SelectedVariant;
    QuestionAnswerSet MySelectedQuestion;

    /// <summary>
    /// Called from start sequence button in game, tells all clients to start the sequence
    /// </summary>
    public override void StartPuzzle()
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

    /// <summary>
    /// get all clients to run the selected puzzle from the current player
    /// </summary>
    /// <param name="Variant">Variant of puzzle sequence</param>
    /// <param name="Question">Question which will be asked</param>
    [PunRPC]
    public void InitializePuzzle(int Variant,int Question)
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

    /// <summary>
    /// Plays the sequence of images followed by the answer board
    /// </summary>
    /// <returns></returns>
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
            MemorySequence.Play();
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

    /// <summary>
    /// Checks if the selected answer is correct
    /// </summary>
    /// <param name="SelectedAnswer"></param>
    public void CheckAnswer(string SelectedAnswer)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber) { return; }

        ClickButton.Play();

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

    /// <summary>
    /// Tells the players if wrong or correct, passes to the next player if correct
    /// </summary>
    /// <param name="NickName"></param>
    /// <param name="Correct"></param>
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

    /// <summary>
    /// Passes to the next player
    /// </summary>
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
