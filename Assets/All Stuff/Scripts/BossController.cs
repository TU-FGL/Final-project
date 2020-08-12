using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private float speedBoss = 5;
    private PlayerController playerControllerScript;
    private int enlargeTreshold=90;
    private int speedUpTreshold=50;
    public bool isCatched;
    public ParticleSystem explosion;

    //boss' components
    private Animator bossAnim;
    private Rigidbody bossRb;
    private AudioSource bossAudio;

    //audio
    public AudioClip speedupSound;
    public AudioClip enlargeSound;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        //boss' components
        bossAnim = GetComponent<Animator>();
        bossRb = GetComponent<Rigidbody>();
        bossAudio = GetComponent<AudioSource>();

        isCatched = true;

        ///gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerScript.gameOver == false)
        {
            if ( playerControllerScript.expPoints == speedUpTreshold)//speeding up the Boss
            {
                ///gameObject.SetActive(true);
                transform.Translate(Vector3.right * Time.deltaTime * speedBoss,Space.World);
                StartCoroutine(BossSpeedUpCountdownRoutine());
                bossAnim.SetFloat("speedMultiplier", 1.5f);
                ///explosion.Play();
            }
            if (playerControllerScript.expPoints == enlargeTreshold) //enlarging the Boss
            {
                transform.localScale *= 2;
                bossAudio.PlayOneShot(enlargeSound, 0.8f);
            }
        }else if (playerControllerScript.gameOver == true && isCatched)
        {
            ///transform.SetPositionAndRotation(transform.position,(0, 90, 0));
            bossAnim.SetTrigger("victoryTrigger");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            playerControllerScript.gameOver = true;

            ///explosionParticle.Play();
            collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right, ForceMode.Impulse);
            bossAnim.SetTrigger("victoryTrigger");

        }
    }

    IEnumerator BossSpeedUpCountdownRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        //transform.localScale /= 1.5f;
        bossAnim.SetFloat("speedMultiplier", 1.0f);
        speedBoss = 0;
        //bossAudio.PlayOneShot(speedupSound, 0.8f);
    }
}
