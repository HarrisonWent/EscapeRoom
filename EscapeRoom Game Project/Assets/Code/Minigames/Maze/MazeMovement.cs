using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMovement : MonoBehaviour
{
    public float  CircleCollider;
    public Vector3 MoveDirection;
    public Transform movePoint;
    public LayerMask Collision;
    public AudioSource MoveClickSound;

    /// <summary>
    /// Moves the maze player target postion if clear
    /// </summary>
    public void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            MoveClickSound.Play();

            if (!Physics2D.OverlapCircle(movePoint.position + MoveDirection, CircleCollider,Collision))
            {
                movePoint.position += MoveDirection;
            }
        }
    }
}
