using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Remaining_Script : MonoBehaviour
{
    private Transform titleTransform; //no use right now, might need it in the future
    private Transform stepsTransform;
    private TextMeshProUGUI stepsTextMesh;
    private int originalSteps;
    private int currentSteps;
    
    private void updateSteps() {
        stepsTextMesh.text = currentSteps.ToString();
    }
    //reset steps to original
    public void resetStep() {
        currentSteps = originalSteps;
        updateSteps();
    }
    //set new origial steps (new question)
    public void setOriginalSteps(int steps) {
        originalSteps = steps;
        resetStep();
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

    // Start is called before the first frame update
    private void Start()
    {
        titleTransform = gameObject.transform.Find("Title");
        stepsTransform = gameObject.transform.Find("Steps");
        stepsTextMesh = stepsTransform.GetComponent<TextMeshProUGUI>();
        setOriginalSteps(3);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
