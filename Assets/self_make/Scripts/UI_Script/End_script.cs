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
    public GameObject End;

    private TextMeshProUGUI title;
    private Text word;
    JsonData fullDialogueData;
    private void changeTitleWordLog()
    {
        string unicodeWord = JsonMapper.ToJson(fullDialogueData["acts"][Static_Variables.currentAct - 1]["content"][Static_Variables.currentPart - 1]["outline"]);
        string wordtext = Regex.Unescape(unicodeWord);
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
        End.gameObject.SetActive(true);
    }
    private void hideEndTitle()
    {
        End.gameObject.SetActive(false);
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
        showEndTitle();
        changeTitleWordLog();
    }
    private void Awake()
    {
        fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Static_Variables.actDirectories[Static_Variables.dialogueMode]));
        End = gameObject.transform.parent.gameObject;
        title = End.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        word = End.transform.GetChild(1).gameObject.GetComponent<Text>();

    }
    void Start()
    {

    }

    void Update()
    {
        
    }
}
