using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score_Script : MonoBehaviour
{
    private TextMeshProUGUI scoreTextMesh;
    private int score;

    //update text mesh
    private void updateScoreText() {
        if (scoreTextMesh != null) {
            scoreTextMesh.text = "Score: " + score.ToString();
        }
    }

    //add n to score
    public void addScore(int n) {
        score += n;
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
    

    private void Awake() {
        scoreTextMesh = gameObject.transform.Find("Score Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        clearScore();
    }

    private void Update() {
        
    }
}
