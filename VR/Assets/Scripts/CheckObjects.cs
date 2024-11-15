using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class CheckObjects : MonoBehaviour
{
    public string targetTag;
    private XRBaseController controller;

    private void Start()
    {
        targetTag = GameManager.instance.selectedTag;
        controller = FindObjectOfType<XRBaseController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == targetTag)
        {
            Debug.Log("Correct tag");
            GameManager.instance.setPoints(GameManager.instance.points += 1);
            ObjectsManager.instance.CheckDespawnTime();
        }
        else if(other.tag != targetTag)
        {
            Debug.Log("Wrong tag");
            controller.SendHapticImpulse(1.0f, 5.0f);
            ObjectsManager.instance.CheckDespawnTime();
        }
    }
}
