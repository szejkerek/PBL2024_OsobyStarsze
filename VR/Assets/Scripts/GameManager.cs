using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{ 
    public static GameManager instance;
    [Header("List of categories")]
    [SerializeField] private string[] categories = { };
    public string selectedTag;
    [Header("Score")]
    public int points;
    [SerializeField] private TMP_Text text;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        selectedTag = categories[Random.Range(0, categories.Length)];
        
        points = 0;
        if(text != null) text.text = points.ToString();
    }

    public int getPoints()
    {
        return points;
    }

    public void setPoints(int p)
    {
        points = p;
        if(text != null) text.text = "Poprawne: " + points.ToString();
    }
}
