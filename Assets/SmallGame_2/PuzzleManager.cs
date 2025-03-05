using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance; // 單例模式

    public GameObject[] pieces; // 存放所有九宮格的物件
    public GameObject winMessage; // 勝利訊息的 UI 物件
    public TextMeshProUGUI timerText; // 倒數計時的 UI 文本
    public GameObject gameOverMessage; // 遊戲結束訊息的 UI 物件

    private bool gameWon = false; // 紀錄遊戲是否完成
    public bool isGameOver = false; // 紀錄遊戲
    private float countdownTime = 30f; // 初始倒數計時時間（秒）

    void Start()
    {
        // 確保遊戲開始時隱藏勝利和遊戲結束訊息
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
        // 確保單例模式
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

        // 更新倒數計時文本
        if (timerText != null)
        {
            timerText.text = $"Time Left: {Mathf.Ceil(countdownTime)}s";
        }

        // 如果倒數計時結束
        if (countdownTime <= 0)
        {
            GameOver();
        }
    }

    public void CheckWinCondition()
    {
        if (gameWon) return; // 如果遊戲已經完成，則不再檢查

        foreach (GameObject piece in pieces)
        {
            float zRotation = piece.transform.eulerAngles.z;

            // 修正浮點數誤差（允許少量偏差）
            if (Mathf.Abs(zRotation % 360) > 1e-2)
            {
                Debug.Log($"Piece {piece.name} not aligned: {zRotation}");
                return; // 如果有任何一塊不正確，直接返回
            }
        }

        // 所有圖片都正確
        gameWon = true; // 完成遊戲結束
        Debug.Log("All pieces aligned! Showing win message...");
        GameOver();
    }

    private void GameOver()
    {
        // 顯示遊戲結束訊息
        if (gameWon)
        {
            winMessage.SetActive(true); // 顯示勝利訊息
        }
        else
        {
            gameOverMessage.SetActive(true);
            gameWon = true;
        }

        isGameOver = true; // 遊戲結束 
        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<PieceController>().enabled = false; // 停用腳本，禁止旋轉
        }
    }
}
