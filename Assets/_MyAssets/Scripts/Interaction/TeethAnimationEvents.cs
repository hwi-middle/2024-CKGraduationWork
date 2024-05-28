using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeethAnimationEvents : MonoBehaviour
{
    public void OnTeethAnimationEnd()
    {
        Destroy(transform.parent.gameObject);
    }
}
