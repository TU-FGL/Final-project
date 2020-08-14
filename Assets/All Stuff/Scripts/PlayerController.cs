using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private BossController bossPlayerInstance;

    //Game Over Text
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public Button exitButton;

    //player's components 
    private Rigidbody playerRb;
    private Animator playerAnim;
    public AudioSource playerAudio;

    //boss
    private Animator bossAnim;
    private GameObject boss;

    //audio
    private AudioSource MainAudio;
    public AudioClip startSound;
    public AudioClip[] jumpSound;
    public AudioClip[] diveSound;
    public AudioClip deathSound;
    public AudioClip[] junkSound;
    public AudioClip[] powerSound;
    public AudioClip[] expSound;
    public AudioClip bossSound;

    public AudioClip victorySound;
    public AudioClip finishSound;

    //forces
    public float jumpForce;
    public float gravityModifier;

    //bools
    public bool isOnGround=true;
    private bool startGame = false;
    public bool gameOver;
    private bool hasPowerup = true;
    private bool isFat = false;
    public bool finish;// whether game is finished/over

    public bool isCan;

    //experience points
    public int expPoints;
    public TextMeshProUGUI expText;

    //treshold of exp when the game speeds up
    private int tresholdSpeedUp=60;

    //Particles
    public ParticleSystem fireworksParticle;
    public ParticleSystem crashParticle;
    public ParticleSystem powerUpParticle;

    //calling other scripts
    private MoveLeft MoveLeft;
    private SpawnManager spawnManagerScript;


    //reaching home
    private int tresholdHome = 100;
    public GameObject home;

    private float pitch = 3f;

    //speed when boss arrives or when has powerup
    private float speedPlayer=5;

    private float leftBorder = 0;
    private float rightBorder = 18f;

    //bool variables to detect if a sound was played
    private bool isBossSoundPlayed = false;
    private bool isFinishSoundPlayed = false;
    private bool isRotated = false;
    private bool isTurnedRight = true;
    private bool isTurnedLeft = false;
    private bool isDeadSoundPlayed = false;


    // Start is called before the first frame update
    void Start()
    {
        bossPlayerInstance = GameObject.Find("Player").GetComponent<BossController>();

        //accessing player's components
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        //accessing others' components
        //MoveLeft = GameObject.Find("Background").GetComponent<MoveLeft>();
        MainAudio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        bossAnim = GameObject.Find("Boss").GetComponent<Animator>();

        ///boss = GameObject.Find("Boss");

        //setting variables
        Physics.gravity *= gravityModifier;
        gameOver = false;
        expPoints = 0;
        finish = false;
        isFat = false;
        hasPowerup = false;

    }

    

    // Update is called once per frame
    void Update()
    {
        if (gameOver || (expPoints == tresholdHome))
        {
            finish = true;
        }

        if (Input.GetKey("escape"))
        {
            ExitGame();
        }

        //playing the sound and start running
        if (!startGame)
        {
            playerAudio.PlayOneShot(startSound, pitch);
            startGame = true;

        }

        if (!gameOver)
        {
            MovePlayer();
        }
        if (gameObject.transform.position.x<leftBorder)
        {
            gameOver = true;
            Death();
        }
        

        if(!gameOver && expPoints == tresholdSpeedUp)
        {
            transform.Translate(Vector3.right * speedPlayer*Time.deltaTime,Space.World);
            StartCoroutine(SpeedUpCountdownRoutine());
            if (!isBossSoundPlayed)
            {
                playerAudio.PlayOneShot(bossSound, pitch); //audio doesnt work
                isBossSoundPlayed = true;
            }
        }
        else if (expPoints == tresholdHome)
        {
            if (!isRotated)
            {
                Debug.Log("won");
                playerRb.transform.Rotate(0, 90, 0);
                isRotated = true;
                restartButton.gameObject.SetActive(true);
                exitButton.gameObject.SetActive(true);
            }

            Instantiate(home, new Vector3(30, 0, 0), home.transform.rotation);
            MainAudio.enabled = false;

            if (!isFinishSoundPlayed)
            {
                playerAudio.PlayOneShot(finishSound, pitch); //audio doesnt work
                isFinishSoundPlayed = true;
            }
            playerAnim.SetTrigger("joyTrigger");
        }


        


    }


    private void OnCollisionEnter(Collision collision)
    {
        //check whether the Player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
        //collision with obstacle
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            Death();

        }
        //getting speed boost
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            
            if (!isFat)
            {
                StartCoroutine(PowerUpCountdownRoutine());
                
                GenerateSound(powerSound);
                Debug.Log("Power Up!");
                ///MoveLeft.speed += 5;
                hasPowerup = true;
                Destroy(collision.gameObject);

                //player
                playerAnim.SetFloat("speedMultiplier", 1.8f);
                if (null != powerUpParticle)
                {

                    powerUpParticle.Play();
                }
                jumpForce += 100;
            }


            Destroy(collision.gameObject);
            
        }
        else if (collision.gameObject.CompareTag("JunkFood"))
        {
            
            StartCoroutine(JunkFoodCountdownRoutine());
            isFat = true;
            GenerateSound(junkSound);
            Debug.Log("Oh no, I will get fat!");

            //player
            jumpForce -= 100;
            transform.localScale *= 1.5f;
            playerAnim.SetFloat("speedMultiplier", 0.8f);

            Destroy(collision.gameObject);



        }

        //getting experience points
        else if (collision.gameObject.CompareTag("Experience"))
        {
            GenerateSound(expSound);
            Debug.Log("Experience!");
            Destroy(collision.gameObject);
            UpdateExp(20);
        }

        //collision with boss
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("Boss touched me!! :-O");
            Death();
        }

    }

    //setting how long powerup will last
    IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        ///MoveLeft.speed -= 5f;

        jumpForce -= 100;
        playerAnim.SetFloat("speedMultiplier", 1.0f);
    }
    IEnumerator JunkFoodCountdownRoutine()
    {
        yield return new WaitForSeconds(4);
        isFat = false;
        jumpForce += 100;
        transform.localScale /= 1.5f;
        playerAnim.SetFloat("speedMultiplier", 1.0f);

    }

    IEnumerator SpeedUpCountdownRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        speedPlayer = 0;
        
    }
    void MovePlayer()
    {
        //jump
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
            playerAnim.SetTrigger("jumpTrigger");
            GenerateSound(jumpSound);

        }

        //divie down
        if (Input.GetKeyDown(KeyCode.DownArrow) && isOnGround)
        {
            GenerateSound(diveSound);
            playerAnim.SetTrigger("diveTrigger");
        }

        //move to right
        if (Input.GetKeyDown(KeyCode.RightArrow) && isOnGround)// && gameObject.transform.position.x < rightBorder && hasPowerup)
        {
            Debug.Log("pushed");
            if (!isTurnedRight)
            {
                Debug.Log("Right" + playerRb.rotation.y);
                playerRb.transform.Rotate(0, 180, 0);
                isTurnedRight = true;
                //isTurnedLeft = false;
            }


            float a = 200f;
            playerRb.AddForce(Vector3.right * a, ForceMode.Acceleration);
        }

        //move to left
        if (Input.GetKeyDown(KeyCode.LeftArrow) && isOnGround)// && gameObject.transform.position.x > leftBorder && hasPowerup)
        {
            if (isTurnedRight)
            {
                Debug.Log("Left" + playerRb.rotation.y);
                playerRb.transform.Rotate(0, 180, 0);
                isTurnedRight = false;
                //isTurnedRight = true;
            }

            float a = 200f;
            playerRb.AddForce(Vector3.left * a, ForceMode.Acceleration);
        }

    }
    void GenerateSound(AudioClip[] sound)
    {
        int ind = Random.Range(0, sound.Length);
        playerAudio.PlayOneShot(sound[ind], pitch);
    }

    private void UpdateExp(int expToAdd)
    {
        expPoints += expToAdd;
        expText.text = "Exp:" + expPoints;
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }

    private void Death()
    {
        gameOver = true;
        GameOver();
        Debug.Log("Game Over!");
        playerAnim.SetTrigger("deathTrigger");
        if (!isDeadSoundPlayed)
        {
            playerAudio.PlayOneShot(deathSound, pitch);
            isDeadSoundPlayed = true;
        }
        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        if (null != crashParticle)
        {

            crashParticle.Play();
        }

    }
   
    private void Win()
    {

        Debug.Log("won");
        playerRb.transform.Rotate(0, 180, 0);
        playerAudio.PlayOneShot(victorySound, pitch);
        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);
        if (null != fireworksParticle)
        {

            fireworksParticle.Play();
        }
    }

}
