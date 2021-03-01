using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonSays : MonoBehaviour
{

    public int NumberOfSteps,CurrentStep,NumberOfInputs;
    public string CurrentSolution, CurrentAttempt;

    public GameObject SolutionObject;

    public Material[] Colours;

    private bool Cycle = true;
    

    void Start()
    {
        CurrentSolution = "";
        CurrentAttempt = "";
        AddStep();
    }

    
    void Update()
    {
        CheckStep();
        if(Cycle)
            StartCoroutine(ShowSolution());
    }

    private void AddStep() 
    {
        int RandomID = Random.Range(0, 3);
        Debug.Log(RandomID);
        CurrentSolution += RandomID.ToString();
        CurrentStep++;
    }

    private void CheckStep() 
    {
        if (NumberOfInputs == CurrentStep) 
        {
            if (string.Compare(CurrentSolution, CurrentAttempt) == 0)
            {
                Debug.Log("Step complete");
                AddStep();
            }
            else 
            {
                Debug.Log("Step incomplete");
            }
            CurrentAttempt = "";
            NumberOfInputs = 0;
        }
    }

    IEnumerator ShowSolution() 
    {
        Cycle = false;
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
        }
        SolutionObject.gameObject.GetComponent<MeshRenderer>().material = Colours[4];
        yield return new WaitForSeconds(1);
        Cycle = true;
    }
}
