using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
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
    public AudioClip finishSound;

    public AudioClip victorySound;

    //forces
    public float jumpForce;
    public float throwForce;
    public float gravityModifier;

    //bools
    public bool isOnGround=true;
    private bool startGame = false;
    public bool gameOver;
    private bool hasPowerup = true;
    private bool isFat = false;

    //experience points
    public int expPoints;
    public TextMeshProUGUI expText;

    //treshold of exp when the game speeds up
    private int tresholdSpeedUp=50;


    ///public ParticleSystem explosionParticle;

    //calling other scripts
    private MoveLeft MoveLeft;
    private SpawnManager spawnManagerScript;


    //reaching home
    private int tresholdHome = 100;
    public GameObject home;

    private float pitch = 3f;
    private float speedPlayer=5;
    private float playerInitialPosx;

    private float leftBorder = 0;
    private float rightBorder = 18f;
    // Start is called before the first frame update
    void Start()
    {
        
        //accessing player's components
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        //accessing others' components
        MoveLeft = GameObject.Find("Background").GetComponent<MoveLeft>();
        MainAudio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        bossAnim = GameObject.Find("Boss").GetComponent<Animator>();

        ///boss = GameObject.Find("Boss");

        //setting variables
        Physics.gravity *= gravityModifier;
        gameOver = false;
        expPoints = 0;


    }

    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            ExitGame();
        }

        //playing the sound and start running
        if (!startGame)
        {
            playerAudio.PlayOneShot(startSound, pitch);
            startGame = true;

        }//else MainAudio.enabled = true;

        if (!gameOver)
        {
            MovePlayer();
        }
        if (gameObject.transform.position.x<leftBorder)
        {
            gameOver = true;
        }
        

        if(!gameOver && expPoints == tresholdSpeedUp)
        {
            transform.Translate(Vector3.right * speedPlayer*Time.deltaTime,Space.World);
            StartCoroutine(SpeedUpCountdownRoutine());
            ///playerAudio.PlayOneShot(bossSound, pitch);
        }
        else if (expPoints == tresholdHome)
        {
            Instantiate(home, new Vector3(30, 0, 0), home.transform.rotation);
            MainAudio.enabled = false;
            ///playerAudio.PlayOneShot(finishSound, pitch);
            playerAnim.SetTrigger("victoryTrigger");
            MoveLeft.speed = 0;
            ///if(gameObject.transform.position.x == home.transform.position.x-3f)
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
                jumpForce += 200;
                //powerupIndicator.gameObject.SetActive(true);
                GenerateSound(powerSound);
                Debug.Log("Power Up!");
                MoveLeft.speed += 6;
                playerAnim.SetFloat("speedMultiplier", 1.8f);
                hasPowerup = true;
            }

            ;
            Destroy(collision.gameObject);
            
        }
        else if (collision.gameObject.CompareTag("JunkFood"))
        {
            if (!hasPowerup)
            {
                StartCoroutine(JunkFoodCountdownRoutine());
                isFat = true;
                GenerateSound(junkSound);
                Debug.Log("Oh no, I will get fat!");
                jumpForce -= 100;
                transform.localScale *= 1.5f;
                playerAnim.SetFloat("speedMultiplier", 0.8f);
            }

            Destroy(collision.gameObject);

        }

        //getting experience points
        else if (collision.gameObject.CompareTag("Experience"))
        {
            GenerateSound(expSound);
            Debug.Log("Experience!");
            Destroy(collision.gameObject);
            UpdateExp(10);
        }

        //collision with boss
        if (collision.gameObject.CompareTag("Boss"))
        {
            Death();
        }

    }

    //setting how long powerup will last
    IEnumerator PowerUpCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerup = false;
        jumpForce -= 200;
        playerAnim.SetFloat("speedMultiplier", 1.0f);
        //powerupIndicator.gameObject.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.RightArrow)&&isOnGround && gameObject.transform.position.x < rightBorder && hasPowerup)
        {
            float a = 300f;
            playerRb.AddForce(Vector3.right * a, ForceMode.Acceleration);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && isOnGround && gameObject.transform.position.x > leftBorder && hasPowerup)
        {
            float a = 300f;
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
        playerAudio.PlayOneShot(deathSound, pitch);
        restartButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(true);

    }
   
    private void Win()
    {
        playerAnim.SetTrigger("victoryTrigger");
        playerAudio.PlayOneShot(victorySound, pitch);
    }

}
