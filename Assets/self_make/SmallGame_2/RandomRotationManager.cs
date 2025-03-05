using UnityEngine;

public class RandomRotationManager : MonoBehaviour
{
    // �]�m 9 �Ӥl�Ϥ����ޥ�
    public GameObject[] pieces;

    void Start()
    {
        // �M���Ҧ��l�Ϥ��A�ì��C�ӹϤ��]�m�H������
        foreach (GameObject piece in pieces)
        {
            // �H����ܤ@�Ө��ס]0�X, 90�X, 180�X, 270�X�^
            int randomAngle = Random.Range(0, 4) * 90;
            piece.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        }
    }
}
