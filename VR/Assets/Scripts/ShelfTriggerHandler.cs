using UnityEngine;

public class ShelfTriggerHandler : MonoBehaviour
{
    private OpenShelfDoor parentScript;

    private void Start()
    {
        parentScript = GetComponentInParent<OpenShelfDoor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentScript != null)
        {
            parentScript.Open();
        }
    }
}
