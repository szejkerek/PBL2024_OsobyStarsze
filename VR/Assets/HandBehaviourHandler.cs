using UnityEngine;
using System.Collections.Generic;

public class HandBehaviourHandler : MonoBehaviour
{
    public List<string> targetTags = new List<string>();
    public float destructionDelay = 3f;
    public Vector3 textOffset = new Vector3(0, 2f, 0);

    private void OnTriggerEnter(Collider other)
    {
        if (targetTags.Contains(other.tag))
        {
            CountdownTimer timer = other.GetComponent<CountdownTimer>();
            if (timer == null)
            {
                timer = other.gameObject.AddComponent<CountdownTimer>();
            }
            timer.InitializeTimer(destructionDelay, textOffset);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetTags.Contains(other.tag))
        {
            CountdownTimer timer = other.GetComponent<CountdownTimer>();
            if (timer != null && timer.IsCountingDown)
            {
                Destroy(timer);
            }
        }
    }
}