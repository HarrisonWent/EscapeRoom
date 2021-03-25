using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionOption : MonoBehaviour
{
    public SelectiveMemoryPuzzle MyPuzzle;
    public Text Answertext;
    
    public void SelectMe()
    {
        MyPuzzle.CheckAnswer(Answertext.text);
    }
}
