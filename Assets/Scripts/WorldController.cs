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

    public TMPro.TextMeshProUGUI messageText;

    public int playerScore;
    public int sheepScore;

    public bool gameFinished = false;

    void Start()
    {
        instance = this;

        playerScore = InitialScore;
        sheepScore = InitialScore;
    }

    public void PlayerCut(int amount)
    {
        if(gameFinished) return;
        playerScore -= amount;
        if(playerScore < 0) {
            playerScore = 0;
            messageText.gameObject.active = true;
            messageText.text = "[ Victory ]";
        }
    }

    public void SheepCut(int amount)
    {
        if(gameFinished) return;
        sheepScore -= amount;
        if(sheepScore < 0) {
            sheepScore = 0;
            messageText.gameObject.active = true;
            messageText.text = "[ You Lost To Sheep ]";
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerScoreText.text = playerScore.ToString();
        sheepScoreText.text = sheepScore.ToString();
    }
}
