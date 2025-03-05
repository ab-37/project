using UnityEngine;
using UnityEngine.EventSystems;

public class BackgroundClickHandler : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // �I���I���ɨ����Ҧ��襤���A
        if (PieceController.selectedPiece != null)
        {
            PieceController.selectedPiece.Deselect();
        }
    }
}
