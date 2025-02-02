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

    [Header("Score and Health")]
    public int points;
    public int health = 3; // Player health
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // Keep GameManager persistent

        selectedTag = categories.Length > 0 ? categories[Random.Range(0, categories.Length)] : "";

        points = 0;
        health = 3; // Initialize health correctly
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Poprawne: " + points.ToString();
        if (healthText != null) healthText.text = "Health: " + health.ToString();
    }

    public void IncreaseScore()
    {
        points += 1;
        UpdateUI();
    }

    public void DecreaseHealth()
    {
        if (health > 0) // Prevents negative health values
        {
            health--;
            UpdateUI();
        }

        if (health <= 0)
        {
            Debug.Log("Game Over!");
            // Implement game over logic here
        }
    }

    public int getPoints()
    {
        return points;
    }

    public void setPoints(int p)
    {
        points = p;
        UpdateUI(); // Ensure UI is updated
    }
}
