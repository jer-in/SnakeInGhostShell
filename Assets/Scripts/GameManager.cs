using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int foodEaten = 0;

    public TextMeshProUGUI scoreTxt;
    public GameObject specialFoodPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;
        scoreTxt.text = "Score: " + score;
    }

    public void ResetScore()
    {
        score = 0;
        foodEaten = 0;
        scoreTxt.text = "Score: " + score;
    }

    public void AddFood()
    {
        foodEaten++;

        AddScore(1);

        if (foodEaten % 5 == 0)
        {
            SpawnSpecialFood();
        }
    }

    private void SpawnSpecialFood()
    {
        Vector2 spawnPos = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
        Instantiate(specialFoodPrefab, spawnPos, Quaternion.identity);
    }
}
