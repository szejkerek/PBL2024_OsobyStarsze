using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Tests : MonoBehaviour
{
    private XRBaseController controller;

    public void TestHaptic()
    {
        controller = FindObjectOfType<XRBaseController>();
        controller.SendHapticImpulse(1.0f, 5.0f);
    }
}
