using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public void OnHoverEffectEnter(GameObject button)
    {
        button.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
    }

    public void OnHoverEffectExit(GameObject button)
    {
        button.transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
