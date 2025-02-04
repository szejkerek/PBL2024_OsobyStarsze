using UnityEngine;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public bool IsCountingDown { get; private set; }
    private float remainingTime;
    private float initialDuration;
    private HandBehaviourHandler handHandler;
    private bool isRight;

    public void InitializeTimer(float duration, bool isRight, HandBehaviourHandler handHandler)
    {
        this.handHandler = handHandler;
        this.isRight = isRight;
        initialDuration = duration;
        remainingTime = duration;

        if (!IsCountingDown)
        {
            StartCoroutine(Countdown());
        }
    }

    private IEnumerator Countdown()
    {
        IsCountingDown = true;
        while (remainingTime > 0)
        {
            float progress = 1 - (remainingTime / initialDuration);
            handHandler?.UpdateProgress(progress);
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        HandleTimerCompletion();
    }

    private void HandleTimerCompletion()
    {
        handHandler?.UpdateProgress(1f);
        handHandler?.ResetProgress();

        gameObject.SetActive(false);
        if (GameManager.instance != null)
        {
            if (gameObject.CompareTag(GameManager.instance.selectedTag))
            {
                GameManager.instance.IncreaseScore();
                DataTracker.Instance.currentActionData.handReachedDestination = true;
                DataTracker.Instance.currentActionData.goodTargetFound = true;
                if (isRight)
                {
                    DataTracker.Instance.currentActionData.rightHandReachedDestination = true;
                }
                else
                {
                    DataTracker.Instance.currentActionData.leftHandReachedDestination = true;
                }
            }
            else
            {
                DataTracker.Instance.currentActionData.handReachedDestination = true;
                DataTracker.Instance.currentActionData.goodTargetFound = false;
                if (isRight)
                {
                    DataTracker.Instance.currentActionData.rightHandReachedDestination = true;
                }
                else
                {
                    DataTracker.Instance.currentActionData.leftHandReachedDestination = true;
                }
                GameManager.instance.DecreaseHealth();
            }
        }
        Destroy(this);
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        IsCountingDown = false;
        handHandler?.ResetProgress();
    }

    private void OnDestroy()
    {
        IsCountingDown = false;
        handHandler?.ResetProgress();
    }
}