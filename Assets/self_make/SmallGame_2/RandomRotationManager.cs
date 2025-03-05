using UnityEngine;

public class RandomRotationManager : MonoBehaviour
{
    // 設置 9 個子圖片的引用
    public GameObject[] pieces;

    void Start()
    {
        // 遍歷所有子圖片，並為每個圖片設置隨機旋轉
        foreach (GameObject piece in pieces)
        {
            // 隨機選擇一個角度（0°, 90°, 180°, 270°）
            int randomAngle = Random.Range(0, 4) * 90;
            piece.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        }
    }
}
