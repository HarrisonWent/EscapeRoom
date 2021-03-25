using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxModeSwitch : MonoBehaviour
{
    public Animation BoxSwapAnim;
    public MeshRenderer BoxMesh;
    public Material PlayerMat, HelpMat;
    public AudioSource BoxSwapSound;

    public void SwapBox(bool isPlayer)
    {
        StartCoroutine(DoSwap(isPlayer));
    }

    IEnumerator DoSwap(bool isPlayer)
    {
        BoxSwapSound.Play();
        BoxSwapAnim.Play();
        yield return new WaitForSeconds(BoxSwapAnim.clip.length * 0.5f);

        if (isPlayer)
        {
            BoxMesh.material = PlayerMat;
        }
        else
        {
            BoxMesh.material = HelpMat;
        }
    }

}
