using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Outro_Script : MonoBehaviour
{
    private GameObject levelEndedTextGameObject;
    private Transform levelEndedTextTransform;
    //private TextMeshProUGUI timesUpTextMesh;
    private GameObject pressToReturnTextGameObject;

    //hide and show texts
    public void showLevelEndedText() {
        levelEndedTextGameObject.SetActive(true);
    }
    public void hideLevelEndedText() {
        levelEndedTextGameObject.SetActive(false);
    }
    public void showPressToReturnText() {
        pressToReturnTextGameObject.SetActive(true);
    }
    public void hidePressToReturnText() {
        pressToReturnTextGameObject.SetActive(false);
    }

    private void Awake() {
        levelEndedTextTransform = gameObject.transform.Find("Level Ended Text");
        levelEndedTextGameObject = levelEndedTextTransform.gameObject;
        //timesUpTextMesh = timesUpTextTransform.GetComponent<TextMeshProUGUI>();
        pressToReturnTextGameObject = gameObject.transform.Find("Press To Return Text").gameObject;
    }

    private void Start() {
        //hide the objects initially
        hideLevelEndedText();
        hidePressToReturnText();
    }

    private void Update() {
        
    }
}