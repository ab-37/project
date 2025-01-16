using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using LLMUnity;
using System.Threading.Tasks;
using System.Linq;
using System;
using Unity.VisualScripting;
using LLMUnitySamples;
using System.Text.RegularExpressions;
using UnityEditor.U2D.Animation;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Rendering;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.PackageManager.Requests;


public class Test2_1 : MonoBehaviour
{
    [Header("Models")]
    //public RAG rag;
    public TextAsset scriptfile;
    //public TextAsset backgroundText;

    [Header("Character")]
    public LLMCharacter Kate;
    public LLMCharacter Hao;
    public LLMCharacter Eva;

    [Header("UI elements")]
    //public Text AIText;

    private string outputFilePath = "acter.txt";
    private string translateFilepath = "translate.txt";
    private string jsonFilePath = "dialogues.json";
    private string jsonFilePath2 = "dialogues2.json";


    private List<Act> acts;
    private Dictionary<string, LLMCharacter> characters;
    private List<Dialogue> dialogueLog = new List<Dialogue>();
    private int CurrntAct = 1;
    private Script outputfile;
    private Script outputfile2;


    void Start()
    {
       // outputFilePath = "acter.txt";
        //translateFilepath = "translate.txt"; //0108
        //targetLanguage = "zh-TW";
        InitializeFile();
        acts = JsonUtility.FromJson<Script>(scriptfile.text).acts;
        Debug.Log($"Loaded {acts.Count} acts from script.");
        characters = new Dictionary<string, LLMCharacter>
        {
            { "Kate", Kate },
            { "Hao", Hao },
            { "Eva", Eva }
        };
        //dialogueLog = new List<Dialogue>();
        //jsonFilePath = @"C:\Users\USER\unity\My project (5)\dialogues.json";;
        //Script script = JsonConvert.DeserializeObject<Script>(scriptfile.text);
        SaveDialogueLogToJson();
        StartCoroutine(PlayAct(CurrntAct));

    }
    private void InitializeFile()
    {
        try
        {
            File.Delete(outputFilePath);
            File.Delete(translateFilepath);

            File.WriteAllText(outputFilePath, "Dialogue Log\n========================");
            File.WriteAllText(translateFilepath, "Translated Dialogue Log\n========================");

            /*if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }
            if (File.Exists(translateFilepath))
            {
                File.Delete(translateFilepath);
            }

            Translate("en", "ko", "I'm a real gangster.", (translatedText) =>
            {
                    Debug.Log(translatedText);
            });*/
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to initialize file: " + e.Message);
        }
    }

    private IEnumerator PlayAct(int actNumber)
    {
        Act act = GetAct(actNumber);
        if (act == null)
        {
            Debug.LogError($"Act {actNumber} not found!");
            yield break;
        }

        Debug.Log($"Playing Act {act.act_number}: {act.scene_description}");
        WriteResponseToFile($"Playing Act {act.act_number}: {act.scene_description}\n");
        

        foreach (Dialogue dialogue in act.dialogues)
        {
           yield return StartCoroutine(GenerateLineCoroutine(dialogue.character, dialogue.line));
            //yield return new WaitForSeconds(2f);
        }
        if (actNumber < acts.Count)
        {
            CurrntAct++;
            StartCoroutine(PlayAct(CurrntAct));
        }
        else
        {
            Debug.Log("All acts completed!");
            WriteResponseToFile("All acts completed!");
        }
    }
    private IEnumerator GenerateLineCoroutine(string characterName, string originalLine)
    {
        string generatedLine = null;
        Task<string> generateTask = GenerateLine(characterName, originalLine);
        while (!generateTask.IsCompleted)
        {
            yield return null;
        }
        if (generateTask.Status == TaskStatus.RanToCompletion)
        {
            generatedLine = generateTask.Result;
        }
        else
        {
            //Debug.LogError($"Error generating line for {characterName}");
            generatedLine = originalLine; 
        }

        //AIText.text += $"\n{characterName}: {generatedLine}";
        Debug.Log($"{characterName}: {generatedLine}");
        WriteResponseToFile($"{characterName}: {generatedLine}");
        
        Translate("en", "zh-TW",generatedLine, translatedResponse =>
        {
            ReDialogueLogToJson(jsonFilePath2,outputfile2, 1,new Dialogue(characterName, translatedResponse));
        });
        ReDialogueLogToJson(jsonFilePath, outputfile, 1, new Dialogue(characterName, generatedLine));
        //dialogueLog.Add(new Dialogue(characterName, generatedLine));


    }
    private async Task<string> GenerateLine(string characterName, string originalLine)
    {
        //AIText.text = "Generating response...";
        if (!characters.ContainsKey(characterName))
        {
            //Debug.LogError($"Character {characterName} not found!");
            return originalLine;
        }

        //string prompt = $"{originalLine}";
        string response = await characters[characterName].Chat(originalLine);
        return response;

    }
    //update 0108
    public void Translate(string orignal, string target,string txt,Action<string> x)
    {
       if (string.IsNullOrEmpty(orignal) || string.IsNullOrEmpty(target) || string.IsNullOrEmpty(txt)) return;
       StartCoroutine( TranslateCoroutine(orignal,target,txt, x));
    }
    IEnumerator TranslateCoroutine(string original, string target,string txt, Action<string> x)
    {
        string requestUrl = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={original}&tl={target}&dt=t&q={UnityWebRequest.EscapeURL(txt)}";
        //string requestUrl = string.Format(TranslateUrl, new object[] {original,target,txt});

        using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
                x.Invoke(string.Empty);
                yield break;
            }

            try
            {
                JArray jsonResponse = JArray.Parse(webRequest.downloadHandler.text);
                string translatedText = string.Concat(jsonResponse[0].Select(segment => segment[0]?.ToString()));
                x.Invoke(translatedText);
            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON Parsing Error: {ex.Message}");
                x.Invoke("Error parsing translation response.");
            }
        }
    }

    private Act GetAct(int actNumber)
    {
        if (actNumber < 1 || actNumber > acts.Count) return null;
        return acts[actNumber - 1];
    }

    private void WriteResponseToFile(string response)
    {
        try
        {
            File.AppendAllText(outputFilePath, $"{response}\n");

            Translate("en", "zh-TW", response, translatedResponse =>
            {
                File.AppendAllText(translateFilepath, $"{translatedResponse}\n");
                Debug.Log($"Translated: {translatedResponse}");
            });
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }
    private void SaveDialogueLogToJson()
    {
        outputfile = new Script
        {
            acts = acts.Select(act => new Act
            {
                act_number = act.act_number,
                scene_description = act.scene_description,
                dialogues = new List<Dialogue>()
            }).ToList()
        };
        outputfile2 = new Script
        {
            acts = acts.Select(act => new Act
            {
                act_number = act.act_number,
                scene_description = act.scene_description,
                dialogues = new List<Dialogue>()
            }).ToList()
        };
        File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(outputfile, Formatting.Indented));
        File.WriteAllText(jsonFilePath2, JsonConvert.SerializeObject(outputfile2, Formatting.Indented));

        /*if (File.Exists(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, string.Empty);
        }
        else
        {
            File.WriteAllText(jsonFilePath, "{}");
        }*/
    }
    private void ReDialogueLogToJson(string filepath,Script script, int actNumber, Dialogue newDialogue)
    {
        Act targetAct = script.acts.FirstOrDefault(act => act.act_number == actNumber);
        targetAct.dialogues.Add(newDialogue);
        File.WriteAllText(filepath, JsonConvert.SerializeObject(script, Formatting.Indented));
        //Debug.Log($"Dialogues saved !!");
    }

    //void SetAIText(string text)
    //{
    //    AIText.text = text;
    //}
    public void CheckLLM(LLMCaller llmCaller, bool debug)
    {
        if (!llmCaller.remote && llmCaller.llm != null && llmCaller.llm.model == "")
        {
            string error = $"Please select a llm model in the {llmCaller.llm.gameObject.name} GameObject!";
            if (debug) Debug.LogWarning(error);
            else throw new System.Exception(error);
        }
    }
}
[System.Serializable]
public class Act
{
    public int act_number;
    public string scene_description;
    public List<Dialogue> dialogues;
}

[System.Serializable]
public class Dialogue
{
    public string character;
    public string line;

    public Dialogue(string character, string line)
    {
        this.character = character;
        this.line = line;
    }
}
//[System.Serializable]
//public class DialogueLog
//{
//    public List<Dialogue> dialogues;
//}
[System.Serializable]
public class Script
{
    public List<Act> acts;
}