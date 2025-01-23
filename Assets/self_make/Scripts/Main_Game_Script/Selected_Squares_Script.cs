using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Selected_Squares_Script : MonoBehaviour
{
    private GameObject[] tileObject = new GameObject[9];
    private bool[] isSelected = new bool[9];
    //public GameObject clickVFX;
    private GameObject selectVFX;

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

    //doesn't work?
    public void spawnClickVFX(int square)
    {
        if (selectVFX != null){
            GameObject vfx = Instantiate(selectVFX, tileObject[square].transform.position, Quaternion.identity);
            vfx.SetActive(true);
            vfx.transform.SetParent(transform);
            ParticleSystem ps =vfx.GetComponent<ParticleSystem>();
            //bruh
            //ps.Play();
            Destroy(vfx, ps.main.duration);
        }
    }
    
    /*
    public void pathVFX(ref List<int> selectedPositions) {
        foreach (int pos in selectedPositions) {
            spawnClickVFX(pos);
        }
    }
    */
    
    public void allSquareSpawnClickVFX()
    {
        for(int i=0;i<9;i++)
        {
            if (isSelected[i])
                spawnClickVFX(i);
        }
    }

    private void Awake() {
        for (int i = 0 ; i < 9 ; ++i) {
            //get child objects
            tileObject[i] = gameObject.transform.Find(i.ToString()).gameObject;
        }
        selectVFX = transform.Find("Select VFX").gameObject;
    }
    
    private void Start()
    {
        
        deselectAllSquares();
    }

    private void Update()
    {
        
    }
}
