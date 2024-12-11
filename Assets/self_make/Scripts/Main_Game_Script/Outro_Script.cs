using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Outro_Script : MonoBehaviour
{
    private GameObject timesUpTextGameObject;
    private Transform timesUpTextTransform;
    //private TextMeshProUGUI timesUpTextMesh;
    private GameObject pressToReturnTextGameObject;

    //hide and show texts
    public void showTimesUpText() {
        timesUpTextGameObject.SetActive(true);
    }
    public void hideTimesUpText() {
        timesUpTextGameObject.SetActive(false);
    }
    public void showPressToReturnText() {
        pressToReturnTextGameObject.SetActive(true);
    }
    public void hidePressToReturnText() {
        pressToReturnTextGameObject.SetActive(false);
    }

    private void Awake() {
        timesUpTextTransform = gameObject.transform.Find("Times Up Text");
        timesUpTextGameObject = timesUpTextTransform.gameObject;
        //timesUpTextMesh = timesUpTextTransform.GetComponent<TextMeshProUGUI>();
        pressToReturnTextGameObject = gameObject.transform.Find("Press To Return Text").gameObject;
    }

    private void Start() {
        //hide the objects initially
        hideTimesUpText();
        hidePressToReturnText();
    }

    private void Update() {
        
    }
}