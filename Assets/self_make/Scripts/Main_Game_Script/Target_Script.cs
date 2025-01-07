using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Target_Script : MonoBehaviour
{

    private Transform targetTransform;
    private TextMeshProUGUI targetTextMesh;
    private int target;

    public void showText() {
        targetTransform.gameObject.SetActive(true);
    }

    public void hideText() {
        targetTransform.gameObject.SetActive(false);
    }

    private void updateTargetText() {
        if (targetTextMesh != null) {
            targetTextMesh.text = "Target: " + target.ToString();
        }
    }

    public void setTarget(int n) {
        target = n;
        updateTargetText();
    }

    private void Awake() {
        targetTransform = gameObject.transform.Find("Target Text");
        targetTextMesh = targetTransform.GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        hideText();
    }

    private void Update() {

    }
}
