using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Intro_Script : MonoBehaviour
{
    private GameObject countdownTextGameObject;
    private Transform countdownTextTransform;
    private TextMeshProUGUI countdownTextMesh;
    private string[] displayTexts = {"3", "2", "1", "Go!"};

    //hide and show text
    public void showCountdownText() {
        countdownTextGameObject.SetActive(true);
    }
    public void hideCountdownText() {
        countdownTextGameObject.SetActive(false);
    }

    //update the text
    private void updateText(string s) {
        if (countdownTextMesh != null) {
            countdownTextMesh.text = s;
        }
    }

    //update the text by index number
    public void updateTextByIndex(int i) {
        updateText(displayTexts[i]);
    }
    
    public void clearText() {
        updateText("");
    }

    //wait x seconds
    /*
    private IEnumerator waiter(int x) {
        yield return new WaitForSeconds(x);
    }
    
    private void waitSecs(int x) {
        StartCoroutine(waiter(x));
    }
    */

    //full countdown sequence
    /*
    public IEnumerator startCountdown() {
        Debug.Log("Countdown Called");
        //show the object
        countdownTextGameObject.SetActive(true);
        for (int i = 0 ; i < displayTexts.Length ; ++i) {
            updateText(displayTexts[i]);
            yield return new WaitForSeconds(1);
        }
        updateText("");
        //hide the object
        countdownTextGameObject.SetActive(false);
        //return true;
    }
    */

    private void Awake() {
        countdownTextTransform = gameObject.transform.Find("Countdown Text");
        countdownTextGameObject = countdownTextTransform.gameObject;
        countdownTextMesh = countdownTextTransform.GetComponent<TextMeshProUGUI>();
        
        //hide the object initially
        hideCountdownText();
    }

    private void Start() {
        
    }

    private void Update() {
        
    }
}
