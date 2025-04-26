using UnityEngine;
using UnityEngine.EventSystems; // 引入 UI 事件命名空間

public class PieceController : MonoBehaviour, IPointerClickHandler
{
    public static PieceController selectedPiece; // 用於管理全局選中的圖片
    private Color originalColor;

    void Start()
    {
        // 儲存原始顏色
        originalColor = GetComponent<UnityEngine.UI.Image>().color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果已經有選中的圖片，先取消其選中
        if (selectedPiece != null && selectedPiece != this)
        {
            selectedPiece.Deselect();
        }

        // 設置當前圖片為選中狀態
        selectedPiece = this;
        GetComponent<UnityEngine.UI.Image>().color = Color.cyan; // 改變顏色表示選中
    }

    public void Deselect()
    {
        // 恢復原始顏色並取消選中狀態
        GetComponent<UnityEngine.UI.Image>().color = originalColor;

        // 如果是當前選中的圖片，清空選中狀態
        if (selectedPiece == this)
        {
            selectedPiece = null;
        }
    }

    void Update()
    {
        // 當前圖片處於選中狀態時處理旋轉
        if (selectedPiece == this)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RotatePiece(-90);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                RotatePiece(90);
            }
        }
    }

    private void RotatePiece(float angle)
    {
        // 旋轉圖片
        transform.Rotate(0, 0, angle);
        PuzzleManager.Instance.CheckWinCondition();

        if (PuzzleManager.Instance.isGameOver)
        {
            return; // 若遊戲已結束，則不旋轉
        }
    }
}

