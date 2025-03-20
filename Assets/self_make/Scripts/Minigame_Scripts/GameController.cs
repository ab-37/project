using System;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public TMP_Text questionText; // ����D�ت��奻
    public TMP_InputField inputField; // ��J���ת��奻��
    public TMP_Text feedbackText; // ��ܤ��X���奻

    private int[] n = new int[5]; // �ƦC
    private int correctAnswer; // ���T����
    private int remainingAttempts = 3; // �Ѿl���զ���
    private int randomN; // �H������

    void Start()
    {
        GenerateGame();
        inputField.onSubmit.AddListener(OnInputSubmit);
    }

    void OnInputSubmit(string text)
    {
        CheckAnswer(); // ���U Enter ���ե� CheckAnswer ��k
    }

    void GenerateGame()
    {
        Array.Clear(n, 0, n.Length);
        randomN = UnityEngine.Random.Range(0, 5); // �H������
        int gameMode = UnityEngine.Random.Range(0, 2); // 0 �� 1

        if (gameMode == 0)
        {
            GameMode1();
        }
        else
        {
            GameMode2();
        }

        feedbackText.text = "";
        inputField.text = "";
    }

    void GameMode1()
    {
        n[0] = UnityEngine.Random.Range(1, 21); // 1 ~ 20
        n[1] = UnityEngine.Random.Range(21, 41); // 21 ~ 40

        for (int i = 1; i < 4; i++)
        {
            n[i + 1] = n[i] + n[i - 1];
        }

        if (randomN == 0)
        {
            correctAnswer = n[2] - n[1];
            questionText.text = $"? , {n[1]} , {n[2]} , {n[3]} , {n[4]}";
        }
        else if (randomN == 4)
        {
            correctAnswer = n[2] + n[3];
            questionText.text = $"{n[0]} , {n[1]} , {n[2]} , {n[3]} , ?";
        }
        else
        {
            correctAnswer = n[randomN];

            string sequence = "";
            for (int i = 0; i < 5; i++)
            {
                if (i == 4)
                {
                    sequence += $"{n[i]}";

                }
                else if (i != randomN)
                {
                    sequence += $"{n[i]} , ";
                }
                else
                {
                    sequence += "? , ";
                }
            }
            questionText.text = sequence.TrimEnd(' ', ',');
        }
    }

    void GameMode2()
    {
        for (int i = 0; i < 5; i++)
        {
            if (i != 2 && i != 1)
            {
                n[i] = UnityEngine.Random.Range(1, 101);
            }
        }

        int sum;
        while (n[4] == n[0]) n[0] = UnityEngine.Random.Range(1, 101);
        if ((n[0] + n[4]) % 2 != 0) n[0]++;
        sum = n[0] + n[4];

        while (n[3] > sum) n[3] = UnityEngine.Random.Range(1, 101);
        n[1] = sum - n[3];
        n[2] = sum / 2;

        correctAnswer = n[randomN];

        string sequence = "";
        for (int i = 0; i < 5; i++)
        {
            if (i == randomN && i != 4) sequence += "? , ";
            else if (i == randomN) sequence += "?";
            else if (i == 4) sequence += $"{n[i]}";
            else sequence += $"{n[i]} , ";
        }

        questionText.text = sequence.TrimEnd(' ', ',');
    }

    public void CheckAnswer()
    {
        feedbackText.text = "";
        if (int.TryParse(inputField.text, out int input))
        {
            if (input == correctAnswer)
            {
                feedbackText.text = "Correct!";
                feedbackText.color = Color.green;
            }
            else
            {
                remainingAttempts--;
                if (remainingAttempts > 0)
                {
                    feedbackText.text = $"Wrong! Remaining Attempts: {remainingAttempts}";
                    feedbackText.color = Color.red;
                }
                else
                {
                    feedbackText.text = $"Out of attempts! Correct Answer: {correctAnswer}";
                    feedbackText.color = Color.yellow;
                }
            }
        }
        else
        {
            feedbackText.text = "Please enter a valid number!";
        }
    }
}
