using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LitJson;
using System.IO;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.SceneManagement;


public class End_script : MonoBehaviour
{
    [SerializeField]
    private GameObject End;

    private GameObject titleObject;
    private GameObject wordObject;
    private GameObject mmWordObject;

    private TextMeshProUGUI title;
    private Text word;
    JsonData fullDialogueData;

    private bool endFlag;
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
        mmWordObject.SetActive(true);
    }
    private void hideEndTitle()
    {
        //End.gameObject.SetActive(false);
        titleObject.SetActive(false);
        wordObject.SetActive(false);
        mmWordObject.SetActive(false);
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
        endFlag = true;
    }
    private void Awake()
    {
        Static_Variables.isDialogueLoadedEndScript = false;
        End = gameObject.transform.parent.gameObject;
        titleObject = End.transform.GetChild(0).gameObject;
        wordObject = End.transform.GetChild(1).gameObject;
        mmWordObject = End.transform.GetChild(2).gameObject;
        title = titleObject.GetComponent<TextMeshProUGUI>();
        word = wordObject.GetComponent<Text>();
    }
    private void Start()
    {
        if (!Static_Variables.isDialogueLoadedEndScript) {
            TextAsset fullDialogueTextAsset = new TextAsset();
            if (Static_Variables.dialogueMode == 0) {
                fullDialogueTextAsset = Resources.Load<TextAsset>("GameJson/CNact");
                //Debug.Log(Application.dataPath.ToString()); //for testing
                fullDialogueData = JsonMapper.ToObject(fullDialogueTextAsset.ToString());
            }
            else if (Static_Variables.dialogueMode == 2) {
                fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "SaveFile", "CNdialogues.json")));
                //Debug.Log("Last Dialogue WIP...");
            }
            else {
                Debug.Log("Invalid Dialogue Mode");
            }
            //fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Static_Variables.actDirectories[Static_Variables.dialogueMode]));
            Static_Variables.isDialogueLoadedEndScript = true;
        }
        //fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Static_Variables.actDirectories[Static_Variables.dialogueMode]));
        hideEndTitle();
        endFlag = false;
    }

    private void Update()
    {
        if (endFlag) {
            if (Input.GetKeyDown(KeyCode.M)) {
                endFlag = false;
                SceneManager.LoadScene("NPC", LoadSceneMode.Single);
            }
        }
    }
}
