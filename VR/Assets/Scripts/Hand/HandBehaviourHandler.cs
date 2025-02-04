using UnityEngine;
using System.Collections.Generic;

public class HandBehaviourHandler : MonoBehaviour
{
    public bool isRight;
    public List<string> targetTags = new List<string>();
    public float destructionDelay = 3f;
    public Material progressBarMaterial;

    public bool ShouldTrackHandToObjectTime = true;
    public float TimestampToObjectTime = 0f;

    private CountdownTimer currentTimer;

    private void Start()
    {
        if (progressBarMaterial == null)
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer != null) progressBarMaterial = renderer.material;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetTags.Contains(other.tag))
        {
            if (currentTimer != null) currentTimer.StopTimer();

            if (ShouldTrackHandToObjectTime)
            {
                ShouldTrackHandToObjectTime = false;
                TimestampToObjectTime = Time.time;
            }

            CountdownTimer timer = other.GetComponent<CountdownTimer>();
            if (timer == null) timer = other.gameObject.AddComponent<CountdownTimer>();

            timer.InitializeTimer(destructionDelay, isRight, this);
            currentTimer = timer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetTags.Contains(other.tag))
        {
            CountdownTimer timer = other.GetComponent<CountdownTimer>();
            if (timer != null && timer == currentTimer)
            {
                currentTimer.StopTimer();
                currentTimer = null;
                ResetProgress();
            }
        }
    }

    public void UpdateProgress(float progress)
    {
        if (progressBarMaterial != null)
        {
            float progressBorder = 0.5f - (progress * 1.0f);
            progressBarMaterial.SetFloat("_ProgressBorder", progressBorder);
            //Debug.Log(progressBorder);
        }
    }

    public void ResetProgress()
    {
        if (progressBarMaterial != null)
            progressBarMaterial.SetFloat("_ProgressBorder", 0.5f);
    }
}