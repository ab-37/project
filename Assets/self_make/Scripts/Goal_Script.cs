using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Goal_Script : MonoBehaviour
{
    private Transform numberTransform;
    private TextMeshProUGUI numberTextMesh;
    private int goalNumber;

    private void updateGoalNumber() {
        if (numberTextMesh != null) {
            numberTextMesh.text = goalNumber.ToString();
        }
    }
    public void setGoalNumber(int num) {
        goalNumber = num;
        updateGoalNumber();
    }
    public bool isGoal(int num) {
        return goalNumber == num;
    }

    private void Awake() {
        numberTransform = gameObject.transform.Find("Goal Number");
        numberTextMesh = numberTransform.GetComponent<TextMeshProUGUI>();
    }
    
    private void Start()
    {
        //setGoalNumber(23);
    }

    private void Update()
    {
        
    }
}
