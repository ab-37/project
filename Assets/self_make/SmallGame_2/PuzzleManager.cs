using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance; // ��ҼҦ�

    public GameObject[] pieces; // �s��Ҧ��E�c�檺����
    public GameObject winMessage; // �ӧQ�T���� UI ����
    public TextMeshProUGUI timerText; // �˼ƭp�ɪ� UI �奻
    public GameObject gameOverMessage; // �C�������T���� UI ����

    private bool gameWon = false; // �����C���O�_����
    public bool isGameOver = false; // �����C��
    private float countdownTime = 30f; // ��l�˼ƭp�ɮɶ��]��^

    void Start()
    {
        // �T�O�C���}�l�����óӧQ�M�C�������T��
        if (winMessage != null)
        {
            winMessage.SetActive(false);
        }

        if (gameOverMessage != null)
        {
            gameOverMessage.SetActive(false);
        }
    }

    void Awake()
    {
        // �T�O��ҼҦ�
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!gameWon)
        {
            UpdateCountdown();
        }
    }

    void UpdateCountdown()
    {
        countdownTime -= Time.deltaTime;

        // ��s�˼ƭp�ɤ奻
        if (timerText != null)
        {
            timerText.text = $"Time Left: {Mathf.Ceil(countdownTime)}s";
        }

        // �p�G�˼ƭp�ɵ���
        if (countdownTime <= 0)
        {
            GameOver();
        }
    }

    public void CheckWinCondition()
    {
        if (gameWon) return; // �p�G�C���w�g�����A�h���A�ˬd

        foreach (GameObject piece in pieces)
        {
            float zRotation = piece.transform.eulerAngles.z;

            // �ץ��B�I�ƻ~�t�]���\�ֶq���t�^
            if (Mathf.Abs(zRotation % 360) > 1e-2)
            {
                Debug.Log($"Piece {piece.name} not aligned: {zRotation}");
                return; // �p�G������@�������T�A������^
            }
        }

        // �Ҧ��Ϥ������T
        gameWon = true; // �����C������
        Debug.Log("All pieces aligned! Showing win message...");
        GameOver();
    }

    private void GameOver()
    {
        // ��ܹC�������T��
        if (gameWon)
        {
            winMessage.SetActive(true); // ��ܳӧQ�T��
        }
        else
        {
            gameOverMessage.SetActive(true);
            gameWon = true;
        }

        isGameOver = true; // �C������ 
        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<PieceController>().enabled = false; // ���θ}���A�T�����
        }
    }
}
