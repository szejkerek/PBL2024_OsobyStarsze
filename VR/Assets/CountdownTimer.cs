using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

            // Text configuration
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
            // Update text position and rotation every frame
            UpdateTextPosition();
            UpdateTimerDisplay();
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void UpdateTextPosition()
    {
        if (textObject != null)
        {
            // Position text above object in world space
            textObject.transform.position = transform.position + textPositionOffset;

            // Align text with global up vector (always face camera)
            textObject.transform.rotation = Quaternion.LookRotation(
                Camera.main.transform.forward,
                Vector3.up
            );
        }
    }

    private void UpdateTimerDisplay()
    {
        if (textMesh != null)
        {
            textMesh.text = Mathf.FloorToInt(remainingTime).ToString();
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