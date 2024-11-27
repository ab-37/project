using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Outro_Script : MonoBehaviour
{
    private GameObject timesUpTextGameObject;
    private Transform timesUpTextTransform;
    private TextMeshProUGUI timesUpTextMesh;

    public void showTimesUpText() {
        timesUpTextGameObject.SetActive(true);
    }
    public void hideTimesUpText() {
        timesUpTextGameObject.SetActive(false);
    }

    private void Awake() {
        timesUpTextTransform = gameObject.transform.Find("Times Up Text");
        timesUpTextGameObject = timesUpTextTransform.gameObject;
        timesUpTextMesh = timesUpTextTransform.GetComponent<TextMeshProUGUI>();

        //hide the object initially
        hideTimesUpText();
    }

    private void Start() {

    }

    private void Update() {
        
    }
}