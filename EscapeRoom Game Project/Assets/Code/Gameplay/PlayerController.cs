using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private GameObject PuzzleBox;
    private bool CanRotate = true;

    private void Start()
    {
        PuzzleBox = GameObject.FindGameObjectWithTag("Box");
    }

    void Update()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber != PuzzleManager.QuizzedPlayerNumber)
        {
            return;
        }

        MoveBox();
    }

    /// <summary>
    /// Used to rotate the box
    /// </summary>
    void MoveBox()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            if (CanRotate)
                StartCoroutine(RotateBoxHorizontal());

        }

        if (Input.GetButtonDown("Vertical"))
        {
            if (CanRotate)
                StartCoroutine(RotateBoxVertical());
        }

        if (Input.GetKeyDown("r"))
        {
            if (CanRotate)
                StartCoroutine(RollBox());
        }
    }

    /// <summary>
    /// Rotates the box left/right depending on horizontal input
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateBoxHorizontal()
    {
        CanRotate = false;

        Vector3 TargetRotation = PuzzleBox.transform.eulerAngles + new Vector3(0, Input.GetAxisRaw("Horizontal") * 90, 0);

        //Round to the nearest 90, as T in the move (bellow) may not always reach precisley 1f leading to innacruracy over time
        TargetRotation.x = Mathf.Round(TargetRotation.x / 90) * 90;
        TargetRotation.y = Mathf.Round(TargetRotation.y / 90) * 90;
        TargetRotation.z = Mathf.Round(TargetRotation.z / 90) * 90;

        Vector3 StartRotation = PuzzleBox.transform.eulerAngles;

        float T = 0f;
        float speed = 2.6f;
        while (T < 1f)
        {
            PuzzleBox.transform.eulerAngles = Vector3.Lerp(StartRotation, TargetRotation, T);
            yield return 0;
            T += speed * Time.deltaTime;
        }

        yield return new WaitForSeconds(0.2f);
        CanRotate = true;
    }

    /// <summary>
    /// Rotates the box up/down depending on Vertical input
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateBoxVertical()
    {
        CanRotate = false;

        Vector3 TargetRotation = PuzzleBox.transform.eulerAngles + (PuzzleBox.transform.InverseTransformDirection(Vector3.right) * 90 * -Input.GetAxisRaw("Vertical"));

        //Round to the nearest 90, as T in the move (bellow) may not always reach precisley 1f leading to innacruracy over time
        TargetRotation.x = Mathf.Round(TargetRotation.x / 90) * 90;
        TargetRotation.y = Mathf.Round(TargetRotation.y / 90) * 90;
        TargetRotation.z = Mathf.Round(TargetRotation.z / 90) * 90;

        Vector3 StartRotation = PuzzleBox.transform.eulerAngles;

        float T = 0f;
        float speed = 2.6f;
        while (T < 1.000f)
        {
            PuzzleBox.transform.eulerAngles = Vector3.Lerp(StartRotation, TargetRotation, T);
            T += speed * Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(0.2f);
        CanRotate = true;
    }

    /// <summary>
    /// Spins the box clockwise (on the forward axis)
    /// </summary>
    /// <returns></returns>
    IEnumerator RollBox()
    {
        CanRotate = false;

        float speed = 2.6f;
        float rotleft = 90;
        while (rotleft > 0f)
        {
            PuzzleBox.transform.Rotate(Vector3.forward * speed, Space.World);
            rotleft -= speed;
            yield return 0;
        }

        Vector3 TargetRotation = PuzzleBox.transform.eulerAngles;
        Vector3 StartRotation = PuzzleBox.transform.eulerAngles;

        //Round to the nearest 90, as T in the move (bellow) may not always reach precisley 1f leading to innacruracy over time
        TargetRotation.x = Mathf.Round(TargetRotation.x / 90) * 90;
        TargetRotation.y = Mathf.Round(TargetRotation.y / 90) * 90;
        TargetRotation.z = Mathf.Round(TargetRotation.z / 90) * 90;

        float T = 0f;
        speed *= 3;
        while (T < 1.000f)
        {            
            PuzzleBox.transform.eulerAngles = Vector3.Lerp(StartRotation, TargetRotation, T);
            T += speed * Time.deltaTime;
            yield return 0;
        }

        yield return new WaitForSeconds(0.2f);
        CanRotate = true;
    }
}
