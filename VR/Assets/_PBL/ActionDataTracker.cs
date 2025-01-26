using UnityEngine;

public class ActionDataTracker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataTracker.Instance.ResetGameData();
    }

    // Adds an Inspector button
    [ContextMenu("End Action")]
    public void EndActionInspectorButton()
    {
        DataTracker.Instance.EndAction();
    }
}