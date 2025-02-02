using UnityEngine;
using System.Collections.Generic;

public class HandBehaviourHandler : MonoBehaviour
{
    public bool isRight;
    public List<string> targetTags = new List<string>();
    public float destructionDelay = 3f;
    public Vector3 textOffset = new Vector3(0, 2f, 0);

    public bool ShouldTrackHandToObjectTime = true;
    public float TimestampToObjectTime = 0f;
    

    private void OnTriggerEnter(Collider other)
    {
        if (targetTags.Contains(other.tag))
        {
            if (ShouldTrackHandToObjectTime)
            {
                ShouldTrackHandToObjectTime = false;
                TimestampToObjectTime = Time.time;
            }
            
            
            CountdownTimer timer = other.GetComponent<CountdownTimer>();
            if (timer == null)
            {
                timer = other.gameObject.AddComponent<CountdownTimer>();
            }
            timer.InitializeTimer(destructionDelay, textOffset, isRight);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetTags.Contains(other.tag))
        {
            CountdownTimer timer = other.GetComponent<CountdownTimer>();
            if (timer != null && timer.IsCountingDown)
            {
                timer.StopTimer();
            }
        }
    }
}