using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance; // ��ҼҦ�

    private GameObject[] pieces = new GameObject[9]; // �s��Ҧ��E�c�檺����
    private GameObject winMessage; // �ӧQ�T���� UI ����

    public bool isGameOver; // �����C��

    private bool isCoroutinePlaying; //if the change scene coroutine is done

    void Start()
    {
        winMessage = gameObject.transform.parent.Find("WinMessage").gameObject;
        // �T�O�C���}�l�����óӧQ�M�C�������T��
        if (winMessage != null)
        {
            winMessage.SetActive(false);
        }
        isGameOver = false;
        isCoroutinePlaying = false;

        for (int i = 0 ; i < 9 ; ++i) {
            pieces[i] = gameObject.transform.parent.Find("Piece_" + (i + 1).ToString()).gameObject;
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
