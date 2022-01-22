using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallOneShot : MonoBehaviour
{
    public FMODUnity.EventReference oneShot;

    public void OneShot_SwordMiss()
    {
        FMODUnity.RuntimeManager.PlayOneShot(oneShot);

    }
}
