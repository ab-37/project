using LLMUnitySamples;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Fungus;
using UnityEngine.SceneManagement;
using LitJson;
using System.IO;
using UnityEditorInternal;

public class ControlUI : MonoBehaviour
{
    public Flowchart flowchart;
    public string blockSelect;

    private string LoadData;
    private JsonData StageData;

    //the full dialogue data
    private JsonData fullDialogueData;
    
    //the current loaded node's dialogue data
    private JsonData currentNodeDialogue;

    //the current line number of dialogue playing
    private int currentLine;

    //check if dialogue is finished playing
    private bool isDialogueDone;

    private GameObject mainMenu;
    public void hideMainUI()
    {
        mainMenu.SetActive(false);
    }
    public void showMainUI()
    {
        mainMenu.SetActive(true);
    }

    public void chooseStage(int value)
    {
        switch(value)
        {
            case 1:
                flowchart.ExecuteBlock("Stage1-1");
                break;
            case 2:
                flowchart.ExecuteBlock("Stage1-2");
                break;
            case 3:
                flowchart.ExecuteBlock("Stage1-3");
                break;
        }
    }

    //change scence and tell what is stage running
    public void changeScene(string sceneName,string blockRunning)
    {
        SceneManager.LoadScene(sceneName);
        Static_Variables.blockRunning = blockRunning;
    }
    public void gamePlay()
    {
        SceneManager.LoadScene("Gameplay Main");
    }


    private void playBlockStage(string blockName)
    {
        flowchart.ExecuteBlock(blockName);
    }
    
    private JsonData selectStage(string chosen_id)
    {
        for(int i=0;i< StageData["stage"].Count;i++)
        {
            if(chosen_id == StageData["stage"][i]["id"].ToString())
                return StageData["stage"][i];
        }
        return null;
    }
    /*
    private void changeDialogueTest()
    {
        flowchart.SetStringVariable("VarDialogue", "123") ;
    }
    */

    //split dialogue into lines
    private string splitDialogue(string dialogueText, int lineLimit = 60 /*, int startTruncate = 3, int extraLineIncrement = 6 */) {
        string[] words = dialogueText.Split(' ');
        string outString = "";
        int lineLength = 0;
        //int lineNum = 0;
        foreach (string word in words) {
            if (lineLength + word.Length > lineLimit) {
                //line limit reached
                /*
                if (++lineNum >= startTruncate) {
                    //text is smaller and can hold more characters, redo everything
                    return splitDialogue(dialogueText, lineLimit + extraLineIncrement, startTruncate + 1);
                }
                */
                outString += "\n";
                lineLength = 0;
            }
            outString += word + " ";
            lineLength += word.Length + 1;
        }
        return outString;
    }

    private void executeDialogueText(string character, string dialogue)
    {
        string spDialogue = splitDialogue(dialogue);

        flowchart.SetStringVariable("VarDialogue", spDialogue);
        switch (character)
        {
            case "Kate":
                flowchart.ExecuteBlock("KateSaying");
                break;
            case "Eva":
                flowchart.ExecuteBlock("EvaSaying");
                break;
            case "Mysterious Man":
                flowchart.ExecuteBlock("MysteriousManSaying");
                break;
            case "Back":
                flowchart.ExecuteBlock("BackSaying");
                break;
            case "Commander":
                flowchart.ExecuteBlock("CommanderSaying");
                break;
            case "Victor":
                flowchart.ExecuteBlock("VictorSaying");
                break;
        }
    }

    /*private void findBlockText(string act_number)
    {
        for (int i = 0; i < StageData["stage"].Count; i++)
        {
            if (chosen_id == StageData["stage"][i]["id"].ToString())
                return StageData["stage"][i];
        }
        return null;

    }*/

    //load act node
    private bool loadNode(int act, int node) {

        //fetch version
        JsonData versionJson = null;
        bool successFlag = false;
        foreach (JsonData actJson in fullDialogueData["acts"]) {
            if ((int)actJson["version"] == act) {
                versionJson = actJson;
                successFlag = true;
                break;
            }
        }
        if (!successFlag) {
            Debug.Log("Failed to fetch version");
            return false;
        }

        //temp code
        Debug.Log("Scene Description: " + versionJson["scene_description"]);

        //fetch node
        foreach (JsonData contentJson in versionJson["content"]) {
            if ((int)contentJson["node"] == node) {
                currentNodeDialogue = contentJson["dialogues"];
                return true;
            }
        }
        Debug.Log("Failed to fetch node in version " + act.ToString());
        return false;
    }

    //load single dialogue line
    bool loadDialogueLine(int lineNum) {
        if (lineNum >= currentNodeDialogue.Count) {
            return false;
        }
        
        string characterStr = (string)currentNodeDialogue[lineNum]["character"];
        string lineStr = (string)currentNodeDialogue[lineNum]["line"];

        //debug code below, put real code here
        //Debug.Log(characterStr + ": " + lineStr);
        executeDialogueText(characterStr, lineStr);

        return true;
    }

    private void Start()
    {
        LoadData = File.ReadAllText(Application.dataPath + "/self_make/stage/stage.json");
        StageData = JsonMapper.ToObject(LoadData);

        blockSelect = Static_Variables.blockSelect;
        mainMenu = GameObject.Find("MainUI");

        //load dialogue, temp file path
        fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/self_make/Scripts/NPC_script/acts1.json"));

        if (loadNode(2, 1)) {
            Debug.Log("Dialogue loaded successfully");
        }

        isDialogueDone = false;
        currentLine = -1;

        //debug checking line by line
        //for (int i = 0 ; loadDialogueLine(i) ; ++i) {}

        //Debug.Log(currentNodeDialogue[0]["character"]); //debug



        //Debug.Log(StageData["stage"][1]["next_stage"]);
        //Debug.Log(selectStage("11")["next_stage"]);

        //changeDialogueTest();

        /*
        if (blockSelect != "0")
            playBlockStage(blockSelect);
        else
            Debug.Log("no block select");
        */

    }

    private void Update()
    {
        if (isDialogueDone) {
            //do end of dialogue stuff
        } 
        else {
            if (!SayDialog.GetSayDialog().isActiveAndEnabled) {
                if (!loadDialogueLine(++currentLine)) {
                    isDialogueDone = true;
                    Debug.Log("End of dialogue");
                }
            }
        }
    }
}
