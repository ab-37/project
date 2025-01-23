using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score_Script : MonoBehaviour
{
    private Transform scoreTransform;
    private Transform targetTransform;
    private TextMeshProUGUI scoreTextMesh;
    private TextMeshProUGUI targetTextMesh;

    private int score;
    private int target;
    private int mode; //1 = score, 2 = target

    //update text meshes
    private void updateText() {
        if (mode == 1) {
            //score mode
            if (scoreTextMesh != null) {
                scoreTextMesh.text = "Score: " + score.ToString();
            }
        }
        else if (mode == 2) {
            //target mode
            if (targetTextMesh != null) {
                targetTextMesh.text = score.ToString() + " / " + target.ToString();
            }
        }
    }

    //add n to score
    public void addScore(int n) {
        score += n;
        if (score < 0) {
            score = 0;
        }
        updateText();
    }

    //set score to n
    public void setScore(int n) {
        score = n;
        if (score < 0) {
            score = 0;
        }
        updateText();
    }

    //set score to 0
    public void clearScore() {
        score = 0;
        updateText();
    }

    //get score
    public int getScore()
    => score;

    //set target to n
    public void setTarget(int n) {
        target = n;
        if (target < 0) {
            target = 0;
            
        }
        updateText();
    }

    //set mode to score or target
    public void setMode(int m) {
        mode = m;
        if (mode == 1) {
            //score mode
            targetTransform.gameObject.SetActive(false);
            scoreTransform.gameObject.SetActive(true);
        }
        else if (mode == 2) {
            //target mode
            scoreTransform.gameObject.SetActive(false);
            targetTransform.gameObject.SetActive(true);
        }
    }

    private void Awake() {
        scoreTransform = transform.Find("Score");
        scoreTextMesh = scoreTransform.Find("Score Text").GetComponent<TextMeshProUGUI>();
        targetTransform = transform.Find("Target");
        targetTextMesh = targetTransform.Find("Target Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {

    }

    private void Update() {

    }
}
