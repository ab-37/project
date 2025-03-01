using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // 點擊背景時取消所有選中狀態
        if (PieceController.selectedPiece != null)
        {
            PieceController.selectedPiece.Deselect();
        }
    }
}
