using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using System.IO;


namespace LLMUnitySamples
{
    public class MultipleCharactersInteraction
    {
        InputField playerText;
        Text AIText;
        LLMCharacter llmCharacter;

        string outputFilePath;

        public MultipleCharactersInteraction(InputField playerText, Text AIText, LLMCharacter llmCharacter, string characterName)
        {
            this.playerText = playerText;
            this.AIText = AIText;
            this.llmCharacter = llmCharacter;
            outputFilePath = characterName + "_Response.txt";
        }

        public void Start()
        {
            playerText.onSubmit.AddListener(onInputFieldSubmit);
            playerText.Select();
        }

        public void onInputFieldSubmit(string message)
        {
            playerText.interactable = false;
            AIText.text = "...";
            _ = llmCharacter.Chat(message, SetAIText, AIReplyComplete);
        }

        public void SetAIText(string text)
        {
            AIText.text = text;
            //Debug.Log(text);
            WriteResponseToFile(text);
        }

        public void AIReplyComplete()
        {
            playerText.interactable = true;
            playerText.Select();
            playerText.text = "";
        }

        private void WriteResponseToFile(string response)
        {
            try
            {
                if (File.Exists(outputFilePath))
                {
                    File.WriteAllText(outputFilePath, "");  // 清空文件
                }
                using (StreamWriter writer = new StreamWriter(outputFilePath, false))  // 設定為 false，表示不附加內容，直接覆蓋
                {
                    // 寫入新的回應，這樣前面的訊息將被覆蓋
                    writer.WriteLine("AI Response: " + response);
                    writer.WriteLine("--------");  // 用來區分每次回應
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Failed to write to file: " + e.Message);
            }
        }
    }

    public class MultipleCharacters : MonoBehaviour
    {
        public LLMCharacter llmCharacter1;
        public InputField playerText1;
        public Text AIText1;
        MultipleCharactersInteraction interaction1;

        public LLMCharacter llmCharacter2;
        public InputField playerText2;
        public Text AIText2;
        MultipleCharactersInteraction interaction2;

        public LLMCharacter llmCharacter3;
        public InputField playerText3;
        public Text AIText3;
        MultipleCharactersInteraction interaction3;

        void Start()
        {
            interaction1 = new MultipleCharactersInteraction(playerText1, AIText1, llmCharacter1, "Character1");
            interaction2 = new MultipleCharactersInteraction(playerText2, AIText2, llmCharacter2, "Character2");
            interaction3 = new MultipleCharactersInteraction(playerText3, AIText3, llmCharacter3, "Character3");
            interaction1.Start();
            interaction2.Start();
            interaction3.Start();
        }

        public void CancelRequests()
        {
            llmCharacter1.CancelRequests();
            llmCharacter2.CancelRequests();
            llmCharacter3.CancelRequests();

            interaction1.AIReplyComplete();
            interaction2.AIReplyComplete();
            interaction3.AIReplyComplete();
        }

        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
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
}
