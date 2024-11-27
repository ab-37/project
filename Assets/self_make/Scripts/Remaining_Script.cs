using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Remaining_Script : MonoBehaviour
{
    private Transform titleTransform;
    private Transform stepsTransform;
    private TextMeshProUGUI stepsTextMesh;
    private int originalSteps;
    private int currentSteps;
    
    //hide and show text
    public void hideText() {
        titleTransform.gameObject.SetActive(false);
        stepsTransform.gameObject.SetActive(false);
    }
    public void showText() {
        titleTransform.gameObject.SetActive(true);
        stepsTransform.gameObject.SetActive(true);
    }

    //update the text
    private void updateSteps() {
        stepsTextMesh.text = currentSteps.ToString();
    }
    //reset steps to original
    public void resetSteps() {
        currentSteps = originalSteps;
        updateSteps();
    }
    //get current steps number
    public int getCurrentSteps() {
        return currentSteps;
    }
    //set new origial steps (new question)
    public void setOriginalSteps(int steps) {
        originalSteps = steps;
        resetSteps();
    }
    //decrement current steps by 1
    public void decrementStep() {
        --currentSteps;
        updateSteps();
    }
    //check if there are still steps remaining
    public bool hasSteps() {
        return currentSteps > 0;
    }

    private void Awake() {
        titleTransform = gameObject.transform.Find("Title");
        stepsTransform = gameObject.transform.Find("Steps");
        stepsTextMesh = stepsTransform.GetComponent<TextMeshProUGUI>();
        hideText();
    }
    
    private void Start()
    {
        //setOriginalSteps(3);
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
