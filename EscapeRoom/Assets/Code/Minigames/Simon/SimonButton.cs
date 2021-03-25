using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SimonButton : MonoBehaviour
{
    public int ButtonID;
    private SimonSays SimonObject;
    private Vector3 startscale;

    void Start()
    {
        startscale = transform.localScale;
        SimonObject = GetComponentInParent<SimonSays>();
    }


    private void OnMouseOver()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) 
        {
            StartCoroutine(QuickScale());
            SimonObject.CheckStep(char.Parse(ButtonID.ToString()));
        }
    }

    bool animating = false;
    IEnumerator QuickScale()
    {
        if (animating) { yield break; }

        animating = true;

        while (transform.localScale.magnitude > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 5 * Time.deltaTime);
            yield return 0;
        }

        while (transform.localScale.magnitude < startscale.magnitude)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startscale + (Vector3.one * 0.1f), 5 * Time.deltaTime);
            yield return 0;
        }

        animating = false;
    }
}
