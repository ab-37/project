using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer_Script : MonoBehaviour
{
    private float fullTime; //the exact time
    private int second; //the integer part of fullTime
    private int centisecond; //the hundredth part of fullTime (rounded down)
    private bool isTicking; //if the time is ticking
    private float originalTime; //time to set when resetting

    private TextMeshProUGUI timerTextMesh;

    //update textmesh
    private void updateTimerText() {
        if (timerTextMesh != null) {
            string centisecondText = centisecond.ToString();
            if (centisecondText.Length == 1) {
                centisecondText = "0" + centisecondText;
            }
            timerTextMesh.text = second.ToString() + "." + centisecondText;
        }
    }

    //start or resume timer
    public void startTimer() {
        isTicking = true;
    }

    //pause timer
    public void stopTimer() {
        isTicking = false;
    }

    //toggle timer
    public void toggleTimer() {
        isTicking = !isTicking;
    }

    //update the second and centisecond variables
    private void updateTime() {
        second = (int)fullTime;
        centisecond = (int)(((decimal)fullTime % 1) * 100);
        updateTimerText();
    }

    //set timer to 0, stop the timer
    public void clearTime() {
        fullTime = 0f;
        stopTimer();
        updateTime();
    }

    //reset time to original time
    public void resetTime() {
        fullTime = originalTime;
        updateTime();
    }

    //set the time and original time to t, reset time
    public void setTime(float t) {
        originalTime = t;
        resetTime();
    }

    //check if the timer is 0
    public bool isTimeOver() {
        return fullTime <= 0f;
    }

    //do something when timer ends
    private void timerEnded() {
        clearTime();
        Debug.Log("Time's up!");
    }
    
    private void Awake() {
        timerTextMesh = gameObject.transform.Find("Timer Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        setTime(60f);
        startTimer();
    }

    private void Update() {
        if (isTicking) {
            fullTime -= Time.deltaTime;
            if (fullTime <= 0f) {
                //timer ended
                timerEnded();
            }
            else {
                updateTime();
            }
        }
    }
}
