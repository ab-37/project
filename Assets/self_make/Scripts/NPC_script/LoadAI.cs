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
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class LoadAI : MonoBehaviour
{
    [Header("Models")]
    //public RAG rag;
    public TextAsset scriptfile;
    //public TextAsset backgroundText;

    [Header("Character")]
    public LLMCharacter Kate;
    public LLMCharacter Houtai;
    public LLMCharacter Eva;
    public LLMCharacter tran;

    [Header("UI elements")]
    //public Button Normal;
    //public Button AITXT;
    //public Text AIText;

    private string outputFilePath = "acter.txt";
    private string translateFilepath = "translate.txt";
    private string jsonFilePath = "Assets\\self_make\\Scripts\\dialogues.json";
    private string jsonFilePath2 = "Assets\\self_make\\Scripts\\CNdialogues.json";
    private string CNacts = "Assets\\self_make\\Scripts\\NPC_script\\CNact.json";
    //  private string newtrans = "trans2.txt";
    //  private string newdia = "dia3.json";
    private string CharacterName;


    private List<ACT> acts;
    private List<Content> contents;
    private Dictionary<string, LLMCharacter> characters;
    private Dictionary<string, string> NameTrans;
    private int CurrntAct = 0;
    private int CurrentContent = 0;
    //private string CurrentOutline = "";
    private Script outputfile;
    private Script outputfile2;
    //private Script outputfile3;

    public void StartAI()
    {
        //Debug.Log("ok");
        //SceneManager.LoadScene("Dialogue Main", LoadSceneMode.Single);
        StartTransAI();

    }

    void Start()
    {
        InitializeFile();
        if (scriptfile == null)
        {
            Debug.LogError("scriptfile is null! Please assign it in the Inspector.");
            return;
        }
        acts = JsonUtility.FromJson<Script>(scriptfile.text).acts;
        if (acts == null) { Debug.Log("script is null!"); }
        //Debug.Log($"Loaded {acts.Count} acts from script.");
        if (scriptfile != null)
        {
            Debug.Log("scriptfile contents: " + scriptfile.text);
        }
    }


    public void StartTransAI()
    {
        characters = new Dictionary<string, LLMCharacter>
        {
            { "Kate", Kate },
            { "Houtai", Houtai},
            { "Eva", Eva }
        };
        NameTrans = new Dictionary<string, string>
        {
            {"Kate", "凱特" },
            {"Houtai", "浩汰"},
            {"Eva", "伊娃"},
            {"Victor", "維克特"},
            {"Narration", "旁白"},
            {"commander", "指揮官"},
            {"New Dawn City", "新黎明城" },
            {"Ugh", "痾" },
            {"android", "仿生人" },
            {"Cyber ​​Squad", "賽博小隊" },
            {"Law enforcement officer", "執法官員" },
            {"cyborg", "改造人" }
        };
        foreach (var kvp in characters)
        {
            if (kvp.Value == null)
            {
                Debug.LogError($"Character {kvp.Key} is not assigned!");
            }
        }

        SaveDialogueLogToJson();
        //Normal.onClick.AddListener();
        StartCoroutine(PlayAct(CurrntAct, CurrentContent));
    }
    private void InitializeFile()
    {
        try
        {
            File.Delete(outputFilePath);
            File.Delete(translateFilepath);

            File.WriteAllText(outputFilePath, "Dialogue Log\n========================");
            File.WriteAllText(translateFilepath, "Translated Dialogue Log\n========================");
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to initialize file: " + e.Message);
        }
    }

    private IEnumerator PlayAct(int actNumber, int ContentNode)
    {
        ACT act = GetAct2(actNumber);
        contents = act.content;
        if (act == null || contents == null)
        {
            Debug.LogError($"Act {actNumber} not found!");
            yield break;
        }
        WriteResponseToFile($"Playing Act {act.version}: {act.scene_description}\n");
        Debug.Log($"Playing Act {act.version}: {act.scene_description}");

        Content content = GetContent(ContentNode);
        int actualVersion = act.version;
        int actualNode = content.node;

        //CurrentOutline = content.outline;
        if (content == null)
        {
            Debug.LogError($"Act {ContentNode} not found!");
            yield break;
        }
        WriteResponseToFile($"Playing Content {actualNode}\n");
        Debug.Log($"Playing Content {actualNode}");
        foreach (Dialogue dialogue in content.dialogues)
        {
            CharacterName = dialogue.character;
            yield return StartCoroutine(GenerateLineCoroutine(dialogue.character, dialogue.line, actualVersion, actualNode));
            //yield return new WaitForSeconds(2f);
        }

        yield return StartCoroutine(ProcessTranslationsForAct(actualVersion, actualNode));
        if (ContentNode + 1 < contents.Count)
        {
            CurrentContent = ContentNode + 1;
            //StartCoroutine(PlayAct(CurrntAct, CurrentContent));
            StartCoroutine(PlayAct(actNumber, ContentNode + 1));
        }
        else if (actNumber + 1 < acts.Count)
        {

            //CurrntAct = actNumber+1;
            //CurrentContent = 0;
            //Debug.Log($"Current number={contents.Count}");
            CurrntAct = actNumber + 1;
            StartCoroutine(PlayAct(actNumber + 1, 0));
            //StartCoroutine(PlayAct(CurrntAct, CurrentContent));
        }
        else
        {
            //Debug.Log($"Current Act={CurrntAct}, Current Content={CurrentContent}");
            Debug.Log("\n========================\n All acts completed!");
            WriteResponseToFile("\n========================\n All acts completed!");
            //StartCoroutine(FixTranslationErrors());
            SceneManager.LoadScene("Dialogue Main", LoadSceneMode.Single);
        }
    }
    private IEnumerator GenerateLineCoroutine(string characterName, string originalLine, int actVersion, int contentNode)
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
            generatedLine = originalLine;
        }

        Debug.Log($"{characterName}: {generatedLine}");
        WriteResponseToFile($"{generatedLine}");
        ReDialogueLogToJson(jsonFilePath, outputfile, new Dialogue(characterName, generatedLine), actVersion, contentNode);

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
        string prompt = $"Script Line: {originalLine}\n";
        //prompt += $"Script Outline: {CurrentOutline}";
        string response = await characters[characterName].Chat(prompt);
        response = Regex.Replace(response, @"\n\n---", "").Trim();
        response = response.Replace("\"", "");
        return response;

    }
    //update 0108
    public void Translate(string orignal, string target, string txt, Action<string> x)
    {
        if (string.IsNullOrEmpty(orignal) || string.IsNullOrEmpty(target) || string.IsNullOrEmpty(txt)) return;
        StartCoroutine(TranslateCoroutine(orignal, target, txt, x));
    }

    IEnumerator TranslateCoroutine(string original, string target, string txt, Action<string> x)
    {
        string requestUrl = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={original}&tl={target}&dt=t&q={UnityWebRequest.EscapeURL(txt)}";


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

    private IEnumerator ProcessTranslationsForAct(int Actnumber, int ContNumber)
    {
        //List<ACT> Act = JsonUtility.FromJson<Script>(File.ReadAllText(jsonFilePath)).acts;
        //ACT act = Act[Actnumber];
        //Content cont = act.content[ContNumber];
        Script fullScript = JsonUtility.FromJson<Script>(File.ReadAllText(jsonFilePath));
        List<ACT> actList = fullScript.acts;
        ACT act = actList.FirstOrDefault(a => a.version == Actnumber);
        Content cont = act.content.FirstOrDefault(c => c.node == ContNumber);

        for (int i = 0; i < cont.dialogues.Count; i++)
        {
            string originalLine = cont.dialogues[i].line;
            foreach (KeyValuePair<string, string> change in NameTrans)
            {
                originalLine = originalLine.Replace(change.Key, change.Value);
            }
            if (!string.IsNullOrEmpty(originalLine))
            {
                string translated = string.Empty;

                yield return StartCoroutine(TranslateCoroutine("en", "zh-TW", originalLine, result => { translated = result; }));
                cont.dialogues[i].line = translated;

                File.AppendAllText(translateFilepath, $"{cont.dialogues[i].character}: {translated}\n");
                ReDialogueLogToJson(jsonFilePath2, outputfile2, new Dialogue(cont.dialogues[i].character, translated), Actnumber, ContNumber);
            }

        }
    }
    private ACT GetAct2(int version)
    {

        if (version < 0 || version > acts.Count)
        {
            Debug.Log("full to get act");
            return null;
        }
        return acts[version];

    }
    private Content GetContent(int node)
    {
        if (node < 0 || node > contents.Count)
        {
            Debug.Log("full to get act");
            return null;
        }
        return contents[node];
    }

    private void WriteResponseToFile(string response)
    {
        try
        {
            File.AppendAllText(outputFilePath, $"{CharacterName}: {response}\n");
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }
    private void SaveDialogueLogToJson()
    {
        outputfile = JsonConvert.DeserializeObject<Script>(scriptfile.text);
        if (outputfile == null)
        {
            Debug.Log("null!\n");
        }
        foreach (ACT act1 in outputfile.acts)
        {
            if (act1.content == null)
            {
                Debug.LogError($"Act {act1.version} has null content!");
                continue;
            }
            foreach (Content content1 in act1.content)
            {
                if (content1.dialogues == null)
                {
                    Debug.LogError($"Act {act1.version} Content {content1.node} has null dialogues!");
                    continue;
                }
                content1.dialogues.Clear();
            }

        }
        outputfile2 = JsonConvert.DeserializeObject<Script>(scriptfile.text);
        foreach (ACT act in outputfile2.acts)
        {
            foreach (Content content2 in act.content)
            {
                content2.dialogues.Clear();
            }
        }
        File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(outputfile, Formatting.Indented));
        File.WriteAllText(jsonFilePath2, JsonConvert.SerializeObject(outputfile2, Formatting.Indented));
    }
    private void SolveBOM()
    {
        if (!File.Exists(CNacts))
        {
            Debug.LogError($"File not found: {CNacts}");
            return;
        }
        byte[] buffer = File.ReadAllBytes(CNacts);

        byte[] bomBuffer = new byte[] { 0xEF, 0xBB, 0xBF };

        if (buffer.Length >= 3 && buffer[0] == bomBuffer[0] && buffer[1] == bomBuffer[1] && buffer[2] == bomBuffer[2])
        {
            Debug.Log("UTF-8 BOM detected. Removing...");
            buffer = buffer[3..];
        }
        string jsonText = Encoding.UTF8.GetString(buffer);

        try
        {
            outputfile2 = JsonConvert.DeserializeObject<Script>(jsonText);
            if (outputfile2 == null)
            {
                Debug.Log("null!\n");
            }

            //File.WriteAllText(CNacts, jsonText);
            Debug.Log($"Processed JSON saved: {CNacts}");
            File.WriteAllText(jsonFilePath2, JsonConvert.SerializeObject(outputfile2, Formatting.Indented));
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON Parsing Error: {ex.Message}");
        }
    }
    public void NormalJson()
    {
        //SaveDialogueLogToJson();
        Debug.Log("Normal Loading!");
        //outputfile = JsonConvert.DeserializeObject<Script>(CNacts);
        //if (outputfile == null)
        //{
        //    Debug.Log("null!\n");
        //}
        //SolveBOM();
        //File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(outputfile, Formatting.Indented));
        SceneManager.LoadScene("Dialogue Main", LoadSceneMode.Single);
    }
    public void LastJson()
    {
        //outputfile = JsonConvert.DeserializeObject<Script>(scriptfile.text);
        if (jsonFilePath2 == null || string.IsNullOrEmpty(jsonFilePath2))
        {
            Debug.Log("null!\n");
        }
        SceneManager.LoadScene("Dialogue Main", LoadSceneMode.Single);
    }
    private void ReDialogueLogToJson(string filepath, Script script, Dialogue newDialogue, int actVersion, int contentNode)
    {
        ACT targetAct1 = script.acts.FirstOrDefault(act => act.version == actVersion);
        if (targetAct1 != null)
        {
            Content targetContent = targetAct1.content.FirstOrDefault(content => content.node == contentNode);
            targetContent.dialogues.Add(newDialogue);
            File.WriteAllText(filepath, JsonConvert.SerializeObject(script, Formatting.Indented));
        }
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
    private IEnumerator FixTranslationErrors()
    {
        if (tran == null)
        {
            Debug.LogError("Translation AI (tran) is not assigned!");
            yield break;
        }

        if (!File.Exists(translateFilepath) || !File.Exists(jsonFilePath))
        {
            Debug.LogError("Translation or dialogue file missing!");
            yield break;
        }

        string[] translatedLines = File.ReadAllLines(translateFilepath);
        Script translatedScript = JsonConvert.DeserializeObject<Script>(File.ReadAllText(jsonFilePath));

        List<string> fixedTranslations = new List<string>();

        foreach (var act in translatedScript.acts)
        {
            foreach (var content1 in act.content)
            {
                foreach (var dialogue in content1.dialogues)
                {
                    string originalText = dialogue.line;
                    string correctedText = originalText;

                    Task<string> fixTask = tran.Chat(originalText);
                    while (!fixTask.IsCompleted)
                    {
                        yield return null;
                    }
                    if (fixTask.Status == TaskStatus.RanToCompletion)
                    {
                        correctedText = fixTask.Result;
                    }

                    Debug.Log($"Corrected: {correctedText}");
                }
            }
        }
        Debug.Log("Translation corrections saved successfully.");
    }
    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}

[System.Serializable]
public class ACT
{
    public int version;
    public string scene_description;
    public List<Content> content;
}
[System.Serializable]
public class Content
{
    public int node;
    public string outline;
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
public class Script
{
    public List<ACT> acts;
}
