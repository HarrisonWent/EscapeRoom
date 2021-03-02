using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectiveMemoryPuzzle : Puzzlebase
{
    [System.Serializable]
    public struct Variant
    {
        public Sprite ShapeImage;
        public string Question;
        public string[] AnswerOptions;
        public string CorrectAnswer;
    }

    public Variant[] PuzzleVariants;

    public Text QuestionText;
    public Image PlayerView;
    public Transform AnswerBoard;
    public GameObject AnswerButtonPrefab;

    Variant SelectedVariant;

    public override void StartPuzzle()
    {
        return;
        SelectedVariant = PuzzleVariants[Random.Range(0,PuzzleVariants.Length)];
        PlayerView.sprite = SelectedVariant.ShapeImage;
        
        foreach(string s in SelectedVariant.AnswerOptions)
        {
            //todo this
        }
    }

    public void CheckAnswer(string SelectedAnswer)
    {
        if (SelectedAnswer != SelectedVariant.CorrectAnswer)
        {

        }
        else
        {

        }
    }
}
