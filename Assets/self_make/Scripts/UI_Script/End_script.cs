using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEditor;


public class End_script : MonoBehaviour
{
    [SerializeField]
    private GameObject End;

    private GameObject titleObject;
    private GameObject wordObject;

    private TextMeshProUGUI title;
    private Text word;
    JsonData fullDialogueData;
    private void changeTitleWordLog()
    {
        string unicodeWord = JsonMapper.ToJson(fullDialogueData["acts"][Static_Variables.currentAct - 1]["content"][Static_Variables.currentPart - 1]["outline"]);
        string wordtext= unicodeWord;

        if(Static_Variables.dialogueMode==0)
            wordtext = Regex.Unescape(unicodeWord);

        word.text = wordtext;
        if (Static_Variables.currentAct == 9)
        {
            title.text = "Bad End";
        }          
        else if (Static_Variables.currentAct == 5)
            title.text = "Good End";
    }
    private void showEndTitle()
    {
        //End.gameObject.SetActive(true);
        titleObject.SetActive(true);
        wordObject.SetActive(true);
    }
    private void hideEndTitle()
    {
        //End.gameObject.SetActive(false);
        titleObject.SetActive(false);
        wordObject.SetActive(false);
    }
    public void quitGame()
    {
        Application.Quit();
    }
    public void loadCheckPoint()
    {
        hideEndTitle();
        //idk how to load :(
    }
    public void setEnd()
    {
        changeTitleWordLog();
        showEndTitle();
    }
    private void Awake()
    {
        fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Static_Variables.actDirectories[Static_Variables.dialogueMode]));
        End = gameObject.transform.parent.gameObject;
        titleObject = End.transform.GetChild(0).gameObject;
        wordObject = End.transform.GetChild(1).gameObject;
        title = titleObject.GetComponent<TextMeshProUGUI>();
        word = wordObject.GetComponent<Text>();
        hideEndTitle();
    }
    void Start()
    {

    }

    void Update()
    {
        
    }
}
