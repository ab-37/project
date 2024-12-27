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
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class N2character : MonoBehaviour
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

    private string filePath;
    private List<Act> acts;
    private int CurrntAct = 1;
    private Dictionary<string, LLMCharacter> characters;
    private List<Dialogue> dialogueLog;
    string outputFilePath;
    private string jsonFilePath;

    void Start()
    {
        outputFilePath = "acter.txt";
        InitializeFile();
        acts = JsonUtility.FromJson<Script>(scriptfile.text).acts;
        Debug.Log($"Loaded {acts.Count} acts from script.");
        characters = new Dictionary<string, LLMCharacter>
        {
            { "Kate", Kate },
            { "Hao", Hao },
            { "Eva", Eva }
        };
        dialogueLog = new List<Dialogue>();
        jsonFilePath = "dialogues.json";
        //jsonFilePath = @"C:\Users\USER\unity\My project (5)\dialogues.json";
        Script script = JsonConvert.DeserializeObject<Script>(scriptfile.text);
        Act act = JsonUtility.FromJson<Script>(scriptfile.text).acts[0];
        StartCoroutine(PlayAct(CurrntAct));

    }
    private void InitializeFile()
    {
        try
        {
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            using (StreamWriter writer = new StreamWriter(outputFilePath, false))
            {
                writer.WriteLine("Dialogue Log");
                writer.WriteLine("========================");
            }
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
        WriteResponseToFile($"Playing Act {act.act_number}: {act.scene_description}");

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
            SaveDialogueLogToJson("Game", "All acts completed!");
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
        Debug.Log($"0{characterName}: {generatedLine}");
        WriteResponseToFile($"{characterName}: {generatedLine}");
        SaveDialogueLogToJson(characterName, generatedLine);
        dialogueLog.Add(new Dialogue(characterName, generatedLine));
    }
    private async Task<string> GenerateLine(string characterName, string originalLine)
    {
        //AIText.text = "Generating response...";
        if (!characters.ContainsKey(characterName))
        {
            //Debug.LogError($"Character {characterName} not found!");
            return originalLine;
        }

        string prompt = $"{originalLine}";
        string response = await characters[characterName].Chat(prompt);
        return response;

    }
    private Act GetAct(int actNumber)
    {
        if (actNumber < 1 || actNumber > acts.Count) return null;
        return acts[actNumber - 1];
    }

    //public void LogDialogue(string character, string dialogue)
    //{
    //    string log = $"{character}: {dialogue}\n";
    //    File.AppendAllText(filePath, log);
    //    Debug.Log($"Saved dialogue: {log}");
    //}
    private void WriteResponseToFile(string response)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath, true))
            {
                writer.WriteLine(response);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }
    private void SaveDialogueLogToJson(string response, string actername)
    {
        if (File.Exists(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, string.Empty);
        }
        else
        {
            File.WriteAllText(jsonFilePath, "{}");
        }

        Script outputScript = new Script
        {
            acts = acts.Select(act => new Act
            {
                act_number = act.act_number,
                scene_description = act.scene_description,
                dialogues = act.dialogues.Select(d =>
                {
                    Dialogue updatedDialogue = dialogueLog.FirstOrDefault(dl => dl.character == d.character && dl.line == d.line);
                    Dialogue finalDialogue = updatedDialogue ?? d;
                    //d.character = actername;
                    //d.line = response;
                    return finalDialogue;
                }).ToList()
            }).ToList()
        };
        string json = JsonConvert.SerializeObject(outputScript, Formatting.Indented);
        File.WriteAllText(jsonFilePath, json);
        //Debug.Log($"Dialogues saved to {jsonFilePath}");
    }
    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        //Application.Quit();
    }
    public void CanncelRequest()
    {
        Kate.CancelRequests();
        Hao.CancelRequests();
        Eva.CancelRequests();
    }

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
[System.Serializable]
public class DialogueLog
{
    public List<Dialogue> dialogues;
}
[System.Serializable]
public class Script
{
    public List<Act> acts;
}