using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMovePlayer : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform movePoint;

    /// <summary>
    /// Moves the maze player to the target
    /// </summary>
    public void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }
}
