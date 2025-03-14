using UnityEngine;
using TMPro;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance; // ��ҼҦ�

    public GameObject[] pieces; // �s��Ҧ��E�c�檺����
    public GameObject winMessage; // �ӧQ�T���� UI ����

    public bool isGameOver = false; // �����C��

    void Start()
    {
        // �T�O�C���}�l�����óӧQ�M�C�������T��
        if (winMessage != null)
        {
            winMessage.SetActive(false);
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

    public void CheckWinCondition()
    {
        if (isGameOver) return; // �p�G�C���w�g�����A�h���A�ˬd

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
        isGameOver = true; // �����C������
        Debug.Log("All pieces aligned! Showing win message...");
        GameOver();
    }

    private void GameOver()
    {
        // ��ܹC�������T��
        if (isGameOver)
        {
            winMessage.SetActive(true); // ��ܳӧQ�T��
        }

        isGameOver = true; // �C������ 
        foreach (GameObject piece in pieces)
        {
            piece.GetComponent<PieceController>().enabled = false; // ���θ}���A�T�����
        }
    }
}
