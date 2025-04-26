using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public TMP_Text questionText; // ����D�ت��奻
    public TMP_InputField inputField; // ��J���ת��奻��
    public TMP_Text feedbackText; // ��ܤ��X���奻
    public int gameMode; // �C���Ҧ�    

    private int[] n = new int[5]; // �ƦC
    private int correctAnswer; // ���T����
    private int remainingAttempts = 5; // �Ѿl���զ���
    private int randomN; // �H������

    private bool isGameOver;
    private bool isCoroutinePlaying; //if the change scene coroutine is done

    void Start()
    {
        isGameOver = false;
        isCoroutinePlaying = false;

        GenerateGame();
        inputField.onSubmit.AddListener(OnInputSubmit);
    }

    void Update()
    {
        if (isGameOver) {
            if (!isCoroutinePlaying) {
                isCoroutinePlaying = true;
                StartCoroutine(gameOverCoroutine());
            }
        }   
    }

    private IEnumerator gameOverCoroutine() {
        isCoroutinePlaying = true;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Dialogue Main");
    }

    void OnInputSubmit(string text)
    {
        CheckAnswer(); // ���U Enter ���ե� CheckAnswer ��k
    }

    void GenerateGame()
    {
        Array.Clear(n, 0, n.Length);
        randomN = UnityEngine.Random.Range(0, 5); // �H������
        gameMode = UnityEngine.Random.Range(0, 2); // �C���Ҧ�

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
                feedbackText.text = "Correct! Return in 3s...";
                feedbackText.color = Color.green;
                isGameOver = true;
            }
            else
            {
                remainingAttempts--;
                feedbackText.text = "Wrong!";
                feedbackText.color = Color.red;

                if (remainingAttempts <= 0)
                {
                    if (gameMode == 0)
                    {
                        feedbackText.text = "n[i + 2] = n[i + 1] + n[i] (0 <= n <=2)";
                    }
                    else
                    {
                        feedbackText.text = "2n[2] = n[i] + n[4 - i] (0 <= n <=4)";
                    }
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
