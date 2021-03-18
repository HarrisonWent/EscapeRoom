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

        Vector3 TargetRotation = PuzzleBox.transform.eulerAngles + new Vector3(0,Input.GetAxisRaw("Horizontal") * 90,0);

        //Round to the nearest 90, as T in the move (bellow) may not always reach precisley 1f leading to innacruracy over time
        TargetRotation.x = Mathf.Round(TargetRotation.x / 90) * 90;
        TargetRotation.y = Mathf.Round(TargetRotation.y / 90) * 90;
        TargetRotation.z = Mathf.Round(TargetRotation.z / 90) * 90;

        Vector3 StartRotation = PuzzleBox.transform.eulerAngles;

        float T = 0f;
        float speed = 2.6f;
        while (T < 1f)
        {        
            PuzzleBox.transform.eulerAngles = Vector3.Lerp(StartRotation, TargetRotation,T);
            yield return 0;
            T += speed * Time.deltaTime;
        }

        yield return new WaitForSeconds(0.2f);
        CanRotate = true;
    }

    IEnumerator RotateBoxVertical()
    {
        CanRotate = false;

        Vector3 TargetRotation = PuzzleBox.transform.eulerAngles + (PuzzleBox.transform.InverseTransformDirection(Vector3.right) *90* -Input.GetAxisRaw("Vertical"));
        
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

        //If upside down
        if (Vector3.Angle(PuzzleBox.transform.up,Vector3.up)>90)
        {
            Debug.Log("Upside down");
            T = 0f;
            StartRotation = PuzzleBox.transform.eulerAngles;
            TargetRotation = PuzzleBox.transform.eulerAngles + (PuzzleBox.transform.InverseTransformDirection(Vector3.forward) * 180);
            TargetRotation.x = Mathf.Round(TargetRotation.x / 90) * 90;
            TargetRotation.y = Mathf.Round(TargetRotation.y / 90) * 90;
            TargetRotation.z = Mathf.Round(TargetRotation.z / 90) * 90;
            Debug.Log("Target rotation: " + TargetRotation);
            while (T < 1.1f)
            {                
                PuzzleBox.transform.eulerAngles = Vector3.Lerp(StartRotation, TargetRotation, T);
                T += Time.deltaTime * speed;
                yield return 0;                
            }
        }
        //else if (Vector3.Angle(PuzzleBox.transform.up, Vector3.right) <5)
        //{
        //    Vector3.RotateTowards(PuzzleBox.transform.eulerAngles,)
        //}

        yield return new WaitForSeconds(0.2f);
        CanRotate = true;
    }
}
