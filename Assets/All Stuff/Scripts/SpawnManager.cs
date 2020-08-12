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
    private Vector3 spawnPosGroundObstcle = new Vector3(25, 0, 0);
    ///private Vector3 spawnPosExpPlatform = new Vector3(25, 5, 1);
    private Vector3 spawnPosFood = new Vector3(25, 1, 0);




    //time Delays
    private float starDelayFlyingObs = 3;
    private float starDelayGroundObs = 8;
    private float starDelayPowerUp = 15;
    ///private float starDelayExp = 25;
    private float starDelayJunkFood = 30;
    ///private float starDelayBcg =10;

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



    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnGroundObstacle()
    {
        float starDelay = Random.Range(16, 22);
        int ind = Random.Range(0, groundObstacle.Length);
        if (playerControllerScript.gameOver == false && ind > 0)
        {
            //Instantiate obstacles on the ground
            Instantiate(groundObstacle[ind], spawnPosGroundObstcle, groundObstacle[ind].transform.rotation);
            Invoke("SpawnGroundObstacle", starDelay);

        }
    }
    void SpawnFlyingObstacle()
    {
        float starDelay = Random.Range(9, 13);
        int ind = Random.Range(0, flyingObstacle.Length);
        if (playerControllerScript.gameOver == false)
        {
            //Instantiate flying obstacles 
            GameObject fly = Instantiate(flyingObstacle[ind], new Vector3(25, Random.Range(4f, 4.5f), 0), flyingObstacle[ind].transform.rotation);
            fly.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 0, -1f) * flyingForce, ForceMode.Impulse);
            Invoke("SpawnFlyingObstacle", starDelay);

        }
    }
    void SpawnExpPlatform()
    {
        //float starDelay = Random.Range(9, 13);
        int ind = Random.Range(0, expPlatform.Length);
        if (playerControllerScript.gameOver == false)
        {
            Instantiate(expPlatform[ind], new Vector3(25, Random.Range(3.5f, 5f), 1), expPlatform[ind].transform.rotation);
            //Invoke("SpawnExpPlatform", starDelay);
        }

    }
    void SpawnPowerUp()
    {
        float starDelay = Random.Range(12, 18);
        int ind = Random.Range(0, powerUp.Length);
        if (playerControllerScript.gameOver == false)
        {
            Instantiate(powerUp[ind], spawnPosFood, powerUp[ind].transform.rotation);
            Invoke("SpawnPowerUp", starDelay);
            StartCoroutine("SpawnCountDown");
        }

    }

    void SpawnJunkFood()
    {
        float starDelay = Random.Range(17, 30);
        int ind = Random.Range(0, junkFood.Length);
        if (playerControllerScript.gameOver == false)
        {
            Instantiate(junkFood[ind], spawnPosFood, junkFood[ind].transform.rotation);
            Invoke("SpawnJunkFood", starDelay);
        }
    }


    IEnumerator SpawnCountDown()
    {
        yield return new WaitForSeconds(3);
        SpawnExpPlatform();
    }
}
