using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score_Script : MonoBehaviour
{
    private Transform scoreTransform;
    private TextMeshProUGUI scoreTextMesh;
    private int score;

    public void hideText() {
        scoreTransform.gameObject.SetActive(false);
    }
    public void showText() {
        scoreTransform.gameObject.SetActive(true);
    }

    //update text mesh
    private void updateScoreText() {
        if (scoreTextMesh != null) {
            scoreTextMesh.text = "Score: " + score.ToString();
        }
    }

    //add n to score
    public void addScore(int n) {
        score += n;
        if (score < 0) {
            score = 0;
        }
        updateScoreText();
    }

    //set score to n
    public void setScore(int n) {
        score = n;
        updateScoreText();
    }

    //set score to 0
    public void clearScore() {
        score = 0;
        updateScoreText();
    }

    //get score
    public int getScore() {
        return score;
    }

    private void Awake() {
        scoreTransform = gameObject.transform.Find("Score Text");
        scoreTextMesh = scoreTransform.GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        hideText();
    }

    private void Update() {
        
    }
}
