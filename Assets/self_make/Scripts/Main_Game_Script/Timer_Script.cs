using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer_Script : MonoBehaviour
{
    //text positions and scale for countup and countdown
    private static Vector3[] textPositions = {new Vector3(), new Vector3(40f, -450f, 0), new Vector3(27.5f, 410f, 0)};
    private static Vector3[] textScale = {new Vector3(), new Vector3(1f, 1f, 1f), new Vector3(0.7f, 0.7f, 1f)};


    private float fullTime; //the exact time
    private int second; //the integer part of fullTime
    private int centisecond; //the hundredth part of fullTime (rounded down)
    private bool isTicking; //if the time is ticking
    private float originalTime; //time to set when resetting
    private int timerMode; //timer mode,  1 = countdown, 2 = countup

    private Transform timerTransform;
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

    public void setTimerMode(int mode) {
        timerMode = mode;
        timerTransform.position = textPositions[mode];
        timerTransform.localScale = textScale[mode];
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

    //get time
    public float getTime() 
    => fullTime;
    
    private void Awake() {
        timerTransform = gameObject.transform.Find("Timer Text");
        timerTextMesh = timerTransform.GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        /*
        setTime(60f);
        startTimer();
        */
    }

    private void Update() {
        if (isTicking) {
            if (timerMode == 1) {
                fullTime -= Time.deltaTime;
                if (fullTime <= 0f) {
                    //timer ended
                    timerEnded();
                    return;
                }
            }
            else {
                fullTime += Time.deltaTime;
            }
            updateTime();
        }
    }
}
