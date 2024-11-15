using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    [SerializeField]
    private TMP_Text text;

    private void Start()
    {
        Disable();
    }

    private void Disable()
    {
        text.gameObject.SetActive(false);
    }

    private void Enable()
    {
        text.gameObject.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
       Enable();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Disable();
    }
    
}

