using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeChecker : MonoBehaviour
{
    public GameObject CompleteLight;
    public Material Green;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MazePlayer") 
        {
            CompleteLight.GetComponent<Renderer>().material = Green;
        }
    }
}
