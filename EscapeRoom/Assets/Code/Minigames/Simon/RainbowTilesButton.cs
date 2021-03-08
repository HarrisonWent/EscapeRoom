using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowTilesButton : MonoBehaviour
{
    private RainbowTiles RT;
    public int ButtonID, CurrentColour;
    public Material[] Colours;

    private void Start()
    {
        RT = this.transform.GetComponentInParent<RainbowTiles>();
        if (gameObject.tag != "Check") 
        {
            CurrentColour = Random.Range(0, 4);
            ChangeColour();
        }
    }

    private void Update()
    {
        if (CurrentColour > 3)
        {
            CurrentColour = 0;
            RT.CurrentAttempt[ButtonID] = CurrentColour;
            ChangeColour();
        }
        

    }

    private void OnMouseOver() 
    {
       

        if (Input.GetMouseButtonDown(0)) 
        {
            if (gameObject.tag == "Check")
            {
                RT.CurrentAttemptString = "";
                RT.CheckAttempt();
                return;
            }
            CurrentColour++;
            RT.CurrentAttempt[ButtonID] = CurrentColour;
           
            ChangeColour();
        }

        
    }

    void ChangeColour()
    {
        switch (CurrentColour)
        {
            case 0:
                this.gameObject.GetComponent<MeshRenderer>().material = Colours[0];
                break;
            case 1:
                this.gameObject.GetComponent<MeshRenderer>().material = Colours[1];
                break;
            case 2:
                this.gameObject.GetComponent<MeshRenderer>().material = Colours[2];
                break;
            case 3:
                this.gameObject.GetComponent<MeshRenderer>().material = Colours[3];
                break;
        }
    }
}
