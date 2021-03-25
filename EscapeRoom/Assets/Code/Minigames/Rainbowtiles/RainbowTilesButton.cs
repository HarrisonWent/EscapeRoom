using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RainbowTilesButton : MonoBehaviour
{
    private RainbowTiles RT;
    public int ButtonID, CurrentColour;
    public Material[] Colours;
    public AudioSource ClickSound;

    private Vector3 startscale;
    private void Start()
    {
        startscale = transform.localScale;
        RT = transform.GetComponentInParent<RainbowTiles>();
        if (gameObject.tag != "Check") 
        {
            CurrentColour = Random.Range(0, 4);
            ChangeColour();
        }
    }

    private void OnMouseOver() 
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            if (gameObject.tag == "Check")
            {
                RT.CurrentAttemptString = "";
                RT.CheckAttempt();
                return;
            }

            CurrentColour++;

            if (CurrentColour > 3)
            {
                CurrentColour = 0;
            }            

            ChangeColour();
            StartCoroutine(QuickScale());
            ClickSound.Play();
        }
    }

    bool animating = false;
    IEnumerator QuickScale()
    {
        if (animating) { yield break; }

        animating = true;

        while(transform.localScale.magnitude>0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 5 * Time.deltaTime);
            yield return 0;
        }

        while (transform.localScale.magnitude < startscale.magnitude)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startscale + (Vector3.one*0.1f), 5 * Time.deltaTime);
            yield return 0;
        }

        animating = false;
    }

    void ChangeColour()
    {
        RT.CurrentAttempt[ButtonID] = CurrentColour;

        switch (CurrentColour)
        {
            case 0:
                gameObject.GetComponent<MeshRenderer>().material = Colours[0];
                break;
            case 1:
                gameObject.GetComponent<MeshRenderer>().material = Colours[1];
                break;
            case 2:
                gameObject.GetComponent<MeshRenderer>().material = Colours[2];
                break;
            case 3:
                gameObject.GetComponent<MeshRenderer>().material = Colours[3];
                break;
        }
    }
}
