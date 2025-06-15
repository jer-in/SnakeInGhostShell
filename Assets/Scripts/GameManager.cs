using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public TextMeshProUGUI scoreTxt;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) {
            Instance = this;
        } else {
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
        scoreTxt.text = "Score: " + score;
    }
}
