using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(CanvasGroup))]
public class LoadingImage : MonoBehaviour {

    private static LoadingImage _instance;
    public static LoadingImage instance
    {
        get 
        {
            if (_instance == null) {
                _instance = FindObjectOfType<LoadingImage>();
                _instance.Init();
            }
            return _instance;
        }
    }

    private Image image;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        Init();
        Hide();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
        transform.DOKill();
        transform.DORotate(Vector3.one, 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutBounce).SetLoops(-1);
    }

    public void Hide() 
    {
        canvasGroup.alpha = 0;
        transform.DOKill();
    }

    public void Init()
    {
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

}
