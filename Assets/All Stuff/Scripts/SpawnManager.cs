using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //things to be spawn
    public GameObject[] groundObstacle;
    public GameObject[] flyingObstacle;
    public GameObject[] powerUp;
    public GameObject[] junkFood;
    public GameObject[] expPlatform;
    public GameObject[] backgrounds;


    //spawning positions
    ///private Vector3 spawnPosFlyingObstacle = new Vector3(25, 0, 0);
    private Vector3 spawnPosGroundObstcle = new Vector3(30, 1, 1.5f);
    ///private Vector3 spawnPosExpPlatform = new Vector3(25, 5, 1);
    ///private float heightFood = Random.Range(1f,3f);
   /// private Vector3 spawnPosFood;



    //time Delays
    private float starDelayFlyingObs = 2;
    private float starDelayGroundObs = 4;
    private float starDelayPowerUp = 8;
    private float starDelayJunkFood = 12;
    private float starDelay = 14;

    //calling other script
    private PlayerController playerControllerScript;

    private float flyingForce = 450;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        Invoke("SpawnFlyingObstacle", starDelayFlyingObs);
        Invoke("SpawnGroundObstacle", starDelayGroundObs);
        Invoke("SpawnPowerUp", starDelayPowerUp);
        Invoke("SpawnJunkFood", starDelayJunkFood);

        ///spawnPosFood = new Vector3(25, heightFood, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnGroundObstacle()
    {
        //float starDelay = 10;// Random.Range(16, 22);
        int ind = Random.Range(0, groundObstacle.Length);
        if (ind > 0 && !playerControllerScript.finish)
        {
            //Instantiate obstacles on the ground
            Instantiate(groundObstacle[ind], spawnPosGroundObstcle, groundObstacle[ind].transform.rotation);
            Invoke("SpawnGroundObstacle", starDelay);

        }
    }
    void SpawnFlyingObstacle()
    {
        //float starDelay = 10;// Random.Range(6, 10);
        int ind = Random.Range(0, flyingObstacle.Length);
        if (!playerControllerScript.finish)
        {
            //Instantiate flying obstacles 
            GameObject fly = Instantiate(flyingObstacle[ind], new Vector3(25, Random.Range(4.3f, 5.8f), 1.5f), flyingObstacle[ind].transform.rotation);
            fly.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -1f) * flyingForce, ForceMode.Impulse);
            fly.GetComponent<Rigidbody>().AddForce(Vector3.left * 100, ForceMode.Impulse);
            Invoke("SpawnFlyingObstacle", starDelay/2);

        }
    }
    void SpawnExpPlatform()
    {
        int ind = Random.Range(0, expPlatform.Length);
        if (!playerControllerScript.finish)
        {
            Instantiate(expPlatform[ind], new Vector3(25, Random.Range(3f, 3.9f), 1.5f), expPlatform[ind].transform.rotation);
        }

    }
    void SpawnPowerUp()
    {
        //float starDelay = 10;//Random.Range(9, 15);
        int ind = Random.Range(0, powerUp.Length);
        if (!playerControllerScript.finish)
        {
            Instantiate(powerUp[ind], new Vector3(25,Random.Range(1f, 3f), 1.5f), powerUp[ind].transform.rotation);
            Invoke("SpawnPowerUp", starDelay);
            StartCoroutine("SpawnCountDown");
        }

    }

    void SpawnJunkFood()
    {
        //float starDelay = 10;// Random.Range(10, 15);
        int ind = Random.Range(0, junkFood.Length);
        if (!playerControllerScript.finish)
        {
            Instantiate(junkFood[ind], new Vector3(25, Random.Range(1f, 3.5f), 1.5f), junkFood[ind].transform.rotation);
            Invoke("SpawnJunkFood", starDelay);
        }
    }


    IEnumerator SpawnCountDown()
    {
        yield return new WaitForSeconds(2);
        SpawnExpPlatform();
    }
}
