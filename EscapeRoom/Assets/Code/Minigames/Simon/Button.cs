using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public int ButtonID;
    private SimonSays SimonObject;

    void Start()
    {
        SimonObject = transform.parent.gameObject.GetComponent<SimonSays>();
    }


    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            SimonObject.CurrentAttempt += ButtonID.ToString();
            SimonObject.NumberOfInputs++;
        }
    }
}
