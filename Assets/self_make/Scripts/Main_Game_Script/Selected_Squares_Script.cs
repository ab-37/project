using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selected_Squares_Script : MonoBehaviour
{
    private GameObject[] tileObject = new GameObject[9];
    private bool[] isSelected = new bool[9];
    public GameObject clickVFX;

    public bool isSquareSelected(int square) {
        return isSelected[square];
    }
    private void updateSquare(int square) {
        tileObject[square].SetActive(isSelected[square]);
    }
    public void deselectSquare(int square) {
        isSelected[square] = false;
        updateSquare(square);
    }
    public void deselectAllSquares() {
        for (int i = 0 ; i < 9 ; ++i) {
            deselectSquare(i);
        }
    }
    public void selectSquare(int square) {
        isSelected[square] = true;
        updateSquare(square);
    }

    
    public void spawnClickVFX(GameObject square)
    {
        if (clickVFX != null){
            var vfx = Instantiate(clickVFX, square.transform.position, Quaternion.identity) as GameObject;
            vfx.transform.SetParent(square.transform);
            var ps =vfx.GetComponent<ParticleSystem>();
            Destroy(vfx, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    }
    public void allSquareSpawnClickVFX()
    {
        for(int i=0;i<9;i++)
        {
            if (isSelected[i])
                spawnClickVFX(tileObject[i]);
        }
    }
    

    private void Awake() {
        for (int i = 0 ; i < 9 ; ++i) {
            //get child objects
            tileObject[i] = gameObject.transform.GetChild(i).gameObject;
        }
    }
    
    private void Start()
    {
        
        deselectAllSquares();
    }

    private void Update()
    {
        
    }
}
