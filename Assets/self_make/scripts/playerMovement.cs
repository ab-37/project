using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Done_Boundray
{
    public float xMin,xMax,yMin,yMax;
}

public class playerMovement : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Done_Boundray boundary;

    public GameObject shot;
    public Transform shotSpawn;
    public float fireRate;
    
    private float nextFire;


    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKey("z") && Time.time>nextFire ){
            nextFire =Time.time+fireRate;
            Instantiate(shot,shotSpawn.position,shotSpawn.rotation);
            //GetComponent<AudioSource>().Play();

        }
    }

    void FixedUpdate() {

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement =new Vector3(moveHorizontal,moveVertical);
        GetComponent<Rigidbody2D>().velocity =movement *speed;

        /*
        GetComponent<Rigidbody2D>().position = new Vector2(
            Mathf.Clamp (GetComponent<Rigidbody2D>().position.x, boundary.xMin,boundary.xMax),
            Mathf.Clamp (GetComponent<Rigidbody2D>().position.y, boundary.yMin,boundary.yMax)
        );
        */

        
    }
}
