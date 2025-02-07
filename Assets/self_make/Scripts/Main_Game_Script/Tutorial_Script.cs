using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial_Script : MonoBehaviour
{
    //all texts
    private Dictionary<string, string> tutorialTexts = new Dictionary<string, string>();
    
    private Transform tutorialTransform;
    private TextMeshProUGUI tutorialTextMesh;

    public void hideText() {
        tutorialTransform.gameObject.SetActive(false);
    }

    public void showText() {
        tutorialTransform.gameObject.SetActive(true);
    }

    //set text with string
    private void updateText(string message) {
        if (tutorialTextMesh != null) {
            tutorialTextMesh.text = message;
        }
    }

    //set text with key, called by other scripts
    public void setText(string textKey) {
        if (!tutorialTexts.ContainsKey(textKey)) {
            //key is not found, throw error
            string errorMessage = "Error: key \"" + textKey + "\" is not found";
            Debug.Log(errorMessage);
            updateText(errorMessage);
            return;
        }
        updateText(tutorialTexts[textKey]);
    }

    private void Awake() {
        tutorialTransform = gameObject.transform.Find("Tutorial Text");
        tutorialTextMesh = tutorialTransform.GetComponent<TextMeshProUGUI>();

        tutorialTexts.Add("notSelect", "Press arrow keys to move, hold Z to select the square");
        tutorialTexts.Add("select", "Create a math expression!\nrelease Z to complete it.");
        tutorialTexts.Add("intermediate", "Reach the goal number by creating another expression!");
        tutorialTexts.Add("outOfMoves", "You\'re out of moves!\nPress R to reset problem or press Space for new problem.");
        tutorialTexts.Add("goal", "Nice job!\nKeep going until you reach the target score below!");
    }

    private void Start() {
        hideText();
    }

    private void Update() {

    }
}
