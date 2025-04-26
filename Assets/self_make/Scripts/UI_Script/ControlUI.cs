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

public class ControlUI : MonoBehaviour
{
    public Flowchart flowchart;
    public string blockSelect;

    /*
    private string LoadData;
    private JsonData StageData;
    */

    //the full dialogue data
    private JsonData fullDialogueData;

    //the dialogue jump data
    private JsonData dialogueJumpData;

    //the levels data
    private JsonData levelsData;
    
    //the current loaded node's dialogue data
    private JsonData currentNodeDialogue;

    //the current line number of dialogue playing
    private int currentLine;

    //check if dialogue is finished playing
    private bool isDialogueDone;

    //check if currently between dialogues
    private bool isBetweenDialogues;

    //design other chracater name
    public Character targetCharacter;

    //variable to load next bg
    private string newBgName;

    //private GameObject mainMenu;

    //public Transform test;


    public BG_Script bgScript;

    /*
    public void hideMainUI()
    {
        mainMenu.SetActive(false);
    }
    public void showMainUI()
    {
        mainMenu.SetActive(true);
    }
    */
    /*
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
    */

    /*
    //change scence and tell what is stage running
    public void changeScene(string sceneName,string blockRunning)
    {
        SceneManager.LoadScene(sceneName);
        Static_Variables.blockRunning = blockRunning;
    }
    */

    public void loadGameplayScene()
    {
        SceneManager.LoadScene("Gameplay Main");
    }


    private void playBlockStage(string blockName)
    {
        flowchart.ExecuteBlock(blockName);
    }
    
    /*
    private JsonData selectStage(string chosen_id)
    {
        for(int i=0;i< StageData["stage"].Count;i++)
        {
            if(chosen_id == StageData["stage"][i]["id"].ToString())
                return StageData["stage"][i];
        }
        return null;
    }
    */
    /*
    private void changeDialogueTest()
    {
        flowchart.SetStringVariable("VarDialogue", "123") ;
    }
    */

    //split dialogue into lines
    /*
    private string splitDialogue(string dialogueText, int lineLimit = 60) {
        string[] words = dialogueText.Split(' ');
        string outString = "";
        int lineLength = 0;
        //int lineNum = 0;
        foreach (string word in words) {
            if (lineLength + word.Length > lineLimit) {
                //line limit reached
                
                outString += "\n";
                lineLength = 0;
            }
            outString += word + " ";
            lineLength += word.Length + 1;
        }
        return outString;
    }
    */

    private void executeDialogueText(string character, string dialogue)
    {
        //string spDialogue = splitDialogue(dialogue);

        flowchart.SetStringVariable("VarDialogue", dialogue);
        
        switch (character)
        {
            case "Kate":
                flowchart.ExecuteBlock("KateSaying");
                break;
            case "Eva":
                flowchart.ExecuteBlock("EvaSaying");
                break;
            case "Houtai":
                flowchart.ExecuteBlock("HoutaiSaying");
                break;
            case "Narration":
                flowchart.ExecuteBlock("NarrationSaying");
                break;
            case "Commander":
                flowchart.ExecuteBlock("CommanderSaying");
                break;
            case "Victor":
                flowchart.ExecuteBlock("VictorSaying");
                break;
            default:
                targetCharacter.SetStandardText(character);
                flowchart.ExecuteBlock("OtherCharacterSaying");
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

    //find target data of target attribute of json
    private JsonData fetchJsonData(JsonData json, string targetAttribute, int targetData) {
        foreach (JsonData childJson in json) {
            if ((int)childJson[targetAttribute] == targetData) {
                return childJson;
            }
        }
        return null;
    }
    private JsonData fetchJsonData(JsonData json, string targetAttribute, string targetData) {
        foreach (JsonData childJson in json) {
            if ((string)childJson[targetAttribute] == targetData) {
                return childJson;
            }
        }
        return null;
    }

    //load act node
    private bool loadNode(int act, int part) {

        //fetch version
        
        JsonData versionJson = fetchJsonData(fullDialogueData["acts"], "version", act);
        
        //bool successFlag = false;
        /*
        foreach (JsonData actJson in fullDialogueData["acts"]) {
            if ((int)actJson["version"] == act) {
                versionJson = actJson;
                successFlag = true;
                break;
            }
        }
        */
        if (versionJson == null) {
            Debug.Log("Failed to fetch version");
            return false;
        }

        //temp code
        Debug.Log("Scene Description: " + versionJson["scene_description"]);

        //fetch node
        /*
        foreach (JsonData contentJson in versionJson["content"]) {
            if ((int)contentJson["node"] == node) {
                currentNodeDialogue = contentJson["dialogues"];
                return true;
            }
        }
        */
        JsonData nodeJson = fetchJsonData(versionJson["content"], "node", part);
        if (nodeJson == null) {
            Debug.Log("Failed to fetch node in version " + act.ToString());
            return false;
        }

        newBgName = (string)fetchJsonData(fetchJsonData(dialogueJumpData["acts"], "act", act)["parts"], "part", part)["bg"];
        currentNodeDialogue = nodeJson["dialogues"];
        Static_Variables.currentAct = act;
        Static_Variables.currentPart = part;
        Debug.Log(act.ToString() + "-" + part.ToString());
        return true;
    }

    //load single dialogue line
    private bool loadDialogueLine(int lineNum) {
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

    //find the "next" object from dialogue_jump.json
    private JsonData findNextFromDialogueJump(int act, int part) {
        JsonData json = fetchJsonData(dialogueJumpData["acts"], "act", act)["parts"];
        if (json == null) {
            Debug.Log("Failed to fetch act");
            return null;
        }
        json = fetchJsonData(json, "part", part)["next"];
        if (json == null) {
            Debug.Log("Failed to fetch part");
            return null;
        }
        return json;
    }

    //check the next dialogue
    private (int, int) nextDialogue() {
        //JsonData json = null;
        //bool successFlag = false;

        //fetch current act and part
        JsonData lastJson = findNextFromDialogueJump(Static_Variables.currentAct, Static_Variables.currentPart);
        
        //JsonData json = fetchJsonData(dialogueJumpData["acts"], "act", Static_Variables.currentAct)["parts"];
        /*
        if (json == null) {
            Debug.Log("Failed to fetch act");
            return (0, 0);
        }
        json = fetchJsonData(json, "part", Static_Variables.currentPart)["next"];
        if (json == null) {
            Debug.Log("Failed to fetch part");
            return (0, 0);
        }
        */
        if (lastJson == null) {
            return (0, 0);
        }

        //check type
        switch ((string)lastJson["type"]) {
            case "dialogue":
            case "smallgame1":
            case "smallgame2":
                return ((int)lastJson["act"], (int)lastJson["part"]);
            case "level":
                JsonData levelJson = fetchJsonData(levelsData["levels"], "level_number", (int)lastJson["level_number"]);
                if ((int)levelJson["type"] == 1) {
                    //countdown
                    if (Static_Variables.lastGameScore >= (int)lastJson["target"]) {
                        Debug.Log("Passed");
                        return ((int)lastJson["pass"]["act"], (int)lastJson["pass"]["part"]);
                    }
                    Debug.Log("Failed");
                    return ((int)lastJson["fail"]["act"], (int)lastJson["fail"]["part"]);
                }
                else {
                    //countup
                    if (Static_Variables.lastGameTime <= (int)lastJson["target"]) {
                        Debug.Log("Passed");
                        return ((int)lastJson["pass"]["act"], (int)lastJson["pass"]["part"]);
                    }
                    Debug.Log("Failed");
                    return ((int)lastJson["fail"]["act"], (int)lastJson["fail"]["part"]);
                }
            default:
                Debug.Log("Failed to fetch dialogue");
                return (0, 0);
        }
    }

    //LOAD THE NEXT DIALOGUE OR LEVEL (COROUTINE)
    private IEnumerator loadNextThingCoroutine() {
        JsonData lastPart = findNextFromDialogueJump(Static_Variables.currentAct, Static_Variables.currentPart);
        flowchart.ExecuteBlock("CleanAllChatacter");
        switch ((string)lastPart["type"]) {
            case "dialogue":
                (int, int) nextPart = nextDialogue();
                if (loadNode(nextPart.Item1, nextPart.Item2)) {
                    Debug.Log("Dialogue " + nextPart.Item1.ToString() + "-" + nextPart.Item2.ToString() + " loaded successfully");
                    isDialogueDone = false;
                    currentLine = -1;
                    yield return new WaitForSeconds(1);
                    bgScript.setBg(newBgName);
                }
                else {
                    Debug.Log("Failed to load dialogue");
                    yield break;
                }
                break;
            case "level":
                //Static_Variables.level_id = (int)lastPart["level_number"];
                Static_Variables.level_id = (int)fetchJsonData(levelsData["levels"], "level_number", (int)lastPart["level_number"])["level_id"];
                Debug.Log("Loading level ID " + Static_Variables.level_id);
                loadGameplayScene();
                break;
            case "smallgame1":
                Debug.Log("Loading small game 1");
                SceneManager.LoadScene("SmallGame_1");
                break;
            case "smallgame2": 
                Debug.Log("Loading small game 2");
                SceneManager.LoadScene("SmallGame_2");
                break;
            case "end":
                //end code here idk
            default:
                Debug.Log("Failed to fetch next dialogue or level");
                break;
        }
        isBetweenDialogues = false;
    }

    private IEnumerator waitCoroutine(int seconds) {
        yield return new WaitForSeconds(seconds);
    }

    private void Awake()
    {
        bgScript = gameObject.transform.Find("BG/BGHandler").GetComponent<BG_Script>();

        //test = gameObject.transform.parent;


        //LoadData = File.ReadAllText(Application.dataPath + "/self_make/stage/stage.json");
        //StageData = JsonMapper.ToObject(LoadData);

        blockSelect = Static_Variables.blockSelect;
        //mainMenu = GameObject.Find("MainUI");

        //load dialogue, temp file path
        //fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/self_make/Scripts/dialogues.json"));
        Static_Variables.isDialogueLoaded = false;
        
        //load other jsons
        dialogueJumpData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/self_make/Scripts/dialogue_jump.json"));
        levelsData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/self_make/Scripts/levels.json"));

        newBgName = "1";
    }

    private void Start()
    {
        //load the dialogue if haven't
        if (!Static_Variables.isDialogueLoaded) {
            fullDialogueData = JsonMapper.ToObject(File.ReadAllText(Static_Variables.actDirectories[Static_Variables.dialogueMode]));
            Static_Variables.isDialogueLoaded = true;
            StartCoroutine(waitCoroutine(1));
        }

        isBetweenDialogues = true;
        isDialogueDone = false;
        (int, int) nextPart = nextDialogue();
        if (loadNode(nextPart.Item1, nextPart.Item2)) {
            bgScript.setBg(newBgName);
            Debug.Log("Dialogue loaded successfully");
            isDialogueDone = false;
            currentLine = -1;
        }
        else {
            Debug.Log("Failed to load dialogue");
        }
        isBetweenDialogues = false;

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
        if (isBetweenDialogues) {
            return;
        }
        if (isDialogueDone) {
            isBetweenDialogues = true;
            StartCoroutine(loadNextThingCoroutine());
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
