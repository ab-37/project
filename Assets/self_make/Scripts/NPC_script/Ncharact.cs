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

    public class Ncharact : TestUI
    {

        [Header("Models")]
        public RAG rag;
        public TextAsset backgroundText;

        private LLMCharacter currentCharacter;
        private Dictionary<string, List<string>> CharacterData;
        public int numRAGResults = 3;
        //List<string> phrases;
        string ragPath = "Ragtest.zip";


        string charactername;

        new async void Start()
        {
            CharacterData = RAGUtils.ReadGutenbergFile(backgroundText.text);
            await CreateEmbeddings();
            SetActiveCharacter(llmCharacter1);
            base.Start();
        }

        public async Task CreateEmbeddings()
        {
            bool loaded = await rag.Load(ragPath);
            if (!loaded)
            {
#if UNITY_EDITOR
                Stopwatch stopwatch = new Stopwatch();
                foreach (var entry in CharacterData)
                {
                    string CharacterName = entry.Key;
                    foreach (string question in entry.Value)
                    {
                        await rag.Add(question, CharacterName);
                    }
                    //PlayerText.text += $"Creating Embeddings (only once)...\n";
                    //stopwatch.Start();
                    //foreach (string phrase in phrases) await rag.Add(phrase);
                    //stopwatch.Stop();
                    //Debug.Log($"embedded {rag.Count()} phrases in {stopwatch.Elapsed.TotalMilliseconds / 1000f} secs");

                }
                rag.Save(ragPath);
#else
                // if in play mode throw an error
                throw new System.Exception("The embeddings could not be found!");
#endif
            }
        }

        protected override void SetActiveCharacter(LLMCharacter character)
        {
            currentCharacter = character;
            AIText.text = $"{currentCharacter.AIName} is now active!";
        }

        protected async override void OnInputFieldSubmit(string question)
        {
            PlayerText.interactable = false;
            AIText.text = "...";
            string prompt = await CreatePrompt(question);
            _ = currentCharacter.Chat(prompt, SetAIText, AIReplyComplete);
            //(string[] similarPhrases, float[] distances) = await rag.Search(message, 1);
            //string similarPhrase = similarPhrases[0];
            //_ = currentCharacter.Chat("Paraphrase the following phrase: " + similarPhrase, SetAIText, AIReplyComplete);
        }
        private async Task<string> CreatePrompt(string question)
        {
            (string[] similarQuestions, _) = await rag.Search(question, numRAGResults, currentCharacter.AIName);

            string answers = "";
            foreach (string q in similarQuestions)
            {
                answers += $"\n- {q}";
            }

            return $"Question: {question}\n\nBackground Knowledge: {answers}";
        }

        public void SetAIText(string text)
        {
            AIText.text = text;
        }

        public void AIReplyComplete()
        {
            PlayerText.interactable = true;
            PlayerText.Select();
            PlayerText.text = "";
        }
        public void CanncelRequest()
        {
            currentCharacter.CancelRequests();
            llmCharacter1.CancelRequests();
            llmCharacter2.CancelRequests();
            llmCharacter3.CancelRequests();
            AIReplyComplete();
        }
        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }

        void CheckLLMs(bool debug)
        {
            CheckLLM(rag.search.llmEmbedder, debug);
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

        bool onValidateWarning = true;
        void OnValidate()
        {
            if (onValidateWarning && !llmCharacter1.remote && llmCharacter1.llm != null && llmCharacter1.llm.model == "")
            {
                Debug.LogWarning($"Please select a model in the {llmCharacter1.llm.gameObject.name} GameObject!");
                onValidateWarning = false;
            }
        }
    }
    public class TestUI : MonoBehaviour
    {

        [Header("Character")]
        public LLMCharacter llmCharacter1;
        public LLMCharacter llmCharacter2;
        public LLMCharacter llmCharacter3;


        [Header("UI elements")]
        public InputField PlayerText;
        public Text AIText;

        [Header("button")]
        public Button NPC1b;
        public Button NPC2b;
        public Button NPC3b;

        protected void Start()
        {
            AddListeners();
        }
        protected virtual void AddListeners()
        {
            NPC1b.onClick.AddListener(() => SetActiveCharacter(llmCharacter1));
            NPC2b.onClick.AddListener(() => SetActiveCharacter(llmCharacter2));
            NPC3b.onClick.AddListener(() => SetActiveCharacter(llmCharacter3));
            PlayerText.onSubmit.AddListener(OnInputFieldSubmit);
        }
        protected virtual void OnInputFieldSubmit(string question) { }
        protected virtual void SetActiveCharacter(LLMCharacter character) { }
    }
