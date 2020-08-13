using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float speedBoss = 5;
    private PlayerController playerControllerScript;
    private int enlargeThreshold=80;
    private int speedUpThreshold=60;

    //Particles
    public ParticleSystem explosionParticle;
    public ParticleSystem enlargeParticle;

    //boss' components
    private Animator bossAnim;
    private Rigidbody bossRb;
    private AudioSource bossAudio;

    //audio
    public AudioClip laughSound;
    public AudioClip koraaSound;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        //boss' components
        bossAnim = GetComponent<Animator>();
        bossRb = GetComponent<Rigidbody>();
        bossAudio = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (playerControllerScript.gameOver == false)
        {

            //speeding up the Boss
            if ( playerControllerScript.expPoints == speedUpThreshold)
            {
                
                transform.Translate(Vector3.right * Time.deltaTime * speedBoss,Space.World);
                StartCoroutine(BossSpeedUpCountdownRoutine());
                bossAnim.SetFloat("speedMultiplier", 1.5f);
                bossAudio.PlayOneShot(koraaSound,1f); //audio doesn't work
                explosionParticle.Play();

            }
            //enlargeing the boss
            if (playerControllerScript.expPoints == enlargeThreshold) //enlarging the Boss
            {
                transform.localScale *= 2;
                bossAudio.PlayOneShot(laughSound,1f); //audio doesn't work
                enlargeParticle.Play();
            }
            if(playerControllerScript.expPoints== 100)
            {
                gameObject.SetActive(false);
            }
        }
        else if (playerControllerScript.gameOver == true)
        {
            bossAnim.SetTrigger("victoryTrigger");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            playerControllerScript.gameOver = true;
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
        
    }
}
