using UnityEngine;
using UnityEngine.EventSystems; // �ޤJ UI �ƥ�R�W�Ŷ�

public class PieceController : MonoBehaviour, IPointerClickHandler
{
    public static PieceController selectedPiece; // �Ω�޲z�����襤���Ϥ�
    private Color originalColor;

    void Start()
    {
        // �x�s��l�C��
        originalColor = GetComponent<UnityEngine.UI.Image>().color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // �p�G�w�g���襤���Ϥ��A��������襤
        if (selectedPiece != null && selectedPiece != this)
        {
            selectedPiece.Deselect();
        }

        // �]�m��e�Ϥ����襤���A
        selectedPiece = this;
        GetComponent<UnityEngine.UI.Image>().color = Color.cyan; // �����C���ܿ襤
    }

    public void Deselect()
    {
        // ��_��l�C��è����襤���A
        GetComponent<UnityEngine.UI.Image>().color = originalColor;

        // �p�G�O��e�襤���Ϥ��A�M�ſ襤���A
        if (selectedPiece == this)
        {
            selectedPiece = null;
        }
    }

    void Update()
    {
        // ��e�Ϥ��B��襤���A�ɳB�z����
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
        // ����Ϥ�
        transform.Rotate(0, 0, angle);
        PuzzleManager.Instance.CheckWinCondition();

        if (PuzzleManager.Instance.isGameOver)
        {
            return; // �Y�C���w�����A�h������
        }
    }
}

