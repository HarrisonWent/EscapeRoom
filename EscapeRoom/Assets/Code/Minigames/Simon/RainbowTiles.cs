using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowTiles : MonoBehaviour
{
    public int[] CurrentAttempt;
    public string CurrentSolution;
    public string CurrentAttemptString;

    public GameObject CompleteObject;
    public Material Green;
    

    private void Start()
    {
        for (int i = 0; i < 12; i++) 
        {
            int RandomID = Random.Range(0, 4);
            Debug.Log(RandomID);
            CurrentSolution += RandomID.ToString();
        }
    }

    private void Update()
    {
        
    }

    public void CheckAttempt() 
    {
        for (int i = 0; i < CurrentAttempt.Length; i++) 
        {
            CurrentAttemptString += CurrentAttempt[i].ToString();
        }

        if (CurrentAttemptString == CurrentSolution) 
        {
            CompleteObject.GetComponent<MeshRenderer>().material = Green;
            //this.gameObject.GetComponent<RainbowTiles>().enabled = false;
            this.enabled = false;
        }
    }
}
