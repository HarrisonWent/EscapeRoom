using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    public float moveSpeed, Distance, CircleCollider;
    public string Button;
    public Transform movePoint;
    public LayerMask Collision;


    public void Update()
    {
        if (Button != "")
            return;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    public void OnMouseOver()
    {
        if (Button == "")
            return;

        if (Input.GetMouseButtonDown(0)) 
        {
            if (Button == "Left") 
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(Distance, 0, 0),CircleCollider,Collision))
                {
                    movePoint.position += new Vector3(Distance, 0, 0);
                }
            }

            if (Button == "Right")
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(-Distance, 0, 0), CircleCollider, Collision))
                    movePoint.position += new Vector3(-Distance, 0, 0);
            }

            if (Button == "Up") 
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0, Distance, 0), CircleCollider, Collision))
                    movePoint.position += new Vector3(0, Distance, 0);
            }

            if (Button == "Down")
            {
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0, -Distance, 0), CircleCollider, Collision))
                    movePoint.position += new Vector3(0, -Distance, 0);
            }
                
        }
    }
}
