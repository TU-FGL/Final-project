using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    public float speed;
    private PlayerController playerControllerScript;
    private float leftBound = -45;
    ///private Vector3 left = new Vector3(-1, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        speed = 7;
    }

    // Update is called once per frame
    void Update()
    {
        //moving bakground
        if (playerControllerScript.gameOver == false) 
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed, Space.World);
        }


        //destroying objects outside boundary 
        if (transform.position.x < leftBound) //&& gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
