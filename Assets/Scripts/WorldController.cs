using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WorldController : MonoBehaviour
{
    public static WorldController instance;

    public int InitialScore = 1000000;

    public TMPro.TextMeshProUGUI playerScoreText;
    public TMPro.TextMeshProUGUI sheepScoreText;

    public int playerScore;
    public int sheepScore;

    void Start()
    {
        instance = this;

        playerScore = InitialScore;
        sheepScore = InitialScore;
    }

    public void PlayerCut(int amount)
    {
        playerScore -= amount;
    }

    public void SheepCut(int amount)
    {
        sheepScore -= amount;
    }

    // Update is called once per frame
    void Update()
    {
        playerScoreText.text = playerScore.ToString();
        sheepScoreText.text = sheepScore.ToString();
    }
}
