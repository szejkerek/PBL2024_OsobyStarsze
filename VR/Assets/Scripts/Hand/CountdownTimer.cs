using UnityEngine;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public bool IsCountingDown { get; private set; }
    private float remainingTime;
    private TextMesh textMesh;
    private GameObject textObject;
    private Vector3 textPositionOffset;

    public void InitializeTimer(float duration, Vector3 offset)
    {
        textPositionOffset = offset;
        remainingTime = duration;
        CreateTextMesh();
        if (!IsCountingDown)
        {
            StartCoroutine(Countdown());
        }
    }

    private void CreateTextMesh()
    {
        if (textObject == null)
        {
            textObject = new GameObject("TimerDisplay");
            textMesh = textObject.AddComponent<TextMesh>();
            textMesh.anchor = TextAnchor.UpperCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = 20;
            textMesh.color = Color.red;
        }
    }

    private IEnumerator Countdown()
    {
        IsCountingDown = true;
        while (remainingTime > 0)
        {
            UpdateTextPosition();
            UpdateTimerDisplay();
            remainingTime -= Time.deltaTime;
            yield return null;
        }
        HandleTimerCompletion();
    }

    private void UpdateTextPosition()
    {
        if (textObject != null)
        {
            textObject.transform.position = transform.position + textPositionOffset;
            textObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        }
    }

    private void UpdateTimerDisplay()
    {
        if (textMesh != null)
        {
            textMesh.text = Mathf.FloorToInt(remainingTime).ToString();
        }
    }

    private void HandleTimerCompletion()
    {
        gameObject.SetActive(false);
        if (GameManager.instance != null)
        {
            if (gameObject.CompareTag(GameManager.instance.selectedTag))
            {
                GameManager.instance.IncreaseScore();
            }
            else
            {
                GameManager.instance.DecreaseHealth();
            }
        }
        Destroy(textObject);
        Destroy(this);
    }

    public void StopTimer()
    {
        StopAllCoroutines();
        IsCountingDown = false;
        if (textObject != null)
        {
            Destroy(textObject);
        }
    }

    private void OnDestroy()
    {
        IsCountingDown = false;
        if (textObject != null)
        {
            Destroy(textObject);
        }
    }
}