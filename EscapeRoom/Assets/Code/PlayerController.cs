using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        MoveBox();
    }

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
    }

    IEnumerator RotateBoxHorizontal() 
    {
        CanRotate = false;
        PuzzleBox.transform.Rotate(0, Input.GetAxisRaw("Horizontal") * 90, 0);
        yield return new WaitForSeconds(1f);
        CanRotate = true;
    }

    IEnumerator RotateBoxVertical()
    {
        CanRotate = false;
        PuzzleBox.transform.Rotate(Input.GetAxisRaw("Vertical") * 90, 0, 0);
        yield return new WaitForSeconds(1f);
        CanRotate = true;
    }
}
