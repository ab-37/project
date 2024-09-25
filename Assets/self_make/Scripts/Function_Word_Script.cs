using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Function_Word_Script : MonoBehaviour
{

    //private Selected_Squares_Script SelectedSquaresScript;
    private TextMeshProUGUI expressionTextMesh;
    private TextMeshProUGUI resultTextMesh;

    private float AutoSetSize(TextMeshProUGUI textMesh, string word)
    {
        return textMesh.fontSize - word.Length * textMesh.fontSize/100;
    }

    public void UpdateWordText(string expression, int result)
    {
        /*
        foreach (Transform child in gameObject.transform)
        {
            int childNameInt = int.Parse(child.name);
            TextMeshProUGUI textMesh = child.GetComponent<TextMeshProUGUI>();


            switch (childNameInt)
            {
                case 0:
                    textMesh.text = expression;
                    textMesh.fontSize = AutoSetSize(textMesh, expression);
                    break;
                case 1:

                    textMesh.text = result.ToString();
                    break;
            }
        }
        */
        expressionTextMesh.text = expression;
        expressionTextMesh.fontSize = AutoSetSize(expressionTextMesh, expression);
        resultTextMesh.text = result.ToString();
    }
    private void Start()
    {
        expressionTextMesh = gameObject.transform.Find("Expression").GetComponent<TextMeshProUGUI>();
        resultTextMesh = gameObject.transform.Find("Result").GetComponent<TextMeshProUGUI>();
        UpdateWordText("", 0);
    }

    private void Update()
    {
        
    }
}
