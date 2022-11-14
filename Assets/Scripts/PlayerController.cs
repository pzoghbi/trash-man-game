using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 3;
    [SerializeField] float jumpForce = 5;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] AudioClip collectSound;
    [SerializeField] AudioClip bombSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip stepSound;
    [SerializeField] Animator scoreAnimator;
    [SerializeField] ParticleSystem jumpParticles;
    [SerializeField] Transform playerModel;
    [SerializeField] TrashSpawner trashSpawner;

    Animator animator;
    AudioSource audioSource;
    int score = 0;
    float maxFeetLength = .85f;
    Rigidbody rb;
    Vector3 directionRotation;

    bool isJumping = false;
    bool isFalling = false;
    bool isRunning = false;
    bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scoreText.text = score.ToString();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        directionRotation = transform.eulerAngles;
    }
    
    // Update is called once per frame
    void Update()
    {
        bool isOnGround = Physics.Raycast(transform.position, Vector3.down, maxFeetLength);
        float controlThrowHorizontal = Input.GetAxis("Horizontal");
        bool controlJump = Input.GetKeyDown(KeyCode.Space);

        // Set Movement Speed
        Vector3 newVelocity = rb.velocity;
        newVelocity.z = -controlThrowHorizontal * movementSpeed;
        rb.velocity = newVelocity;

        // If Moving - Set Target Direction
        if (isMoving = Mathf.Abs(rb.velocity.z) > 0)
		{
            directionRotation.y = Mathf.Sign(rb.velocity.z) > 0 ? 0: 180;
		}

        // Rotate Towards Target Direction
        transform.rotation = 
            Quaternion.RotateTowards(
                Quaternion.Euler(transform.eulerAngles),
                Quaternion.Euler(directionRotation),
                180 * Time.deltaTime * 4
            );
        
        // If On Ground Or Ground Is Detected
        if (isOnGround)
        {
            animator.SetBool("isJumping", false);

            // If Falling, Trigger Falling Animation
            if (isFalling)
            {
                animator.SetTrigger("triggerLanding");
            }

            // If On Ground and Velocity is Downwards -> Disable Falling And Jumping State
            if (rb.velocity.y <= 0)
			{
                isFalling = false;
                isJumping = false;
			} 

            // If Movement Input Set To Running State
            isRunning = Mathf.Abs(controlThrowHorizontal) > 0;

            // If Pressed Jump
            if (controlJump)
		    {
                rb.velocity = jumpForce * Vector3.up;
                animator.SetBool("isJumping", true);
                audioSource.PlayOneShot(jumpSound, 0.5f);
                ParticleSystem particle = Instantiate(jumpParticles, playerModel.position, Quaternion.identity);
                Destroy(particle.gameObject, particle.main.duration);
                isJumping = true;
                isRunning = false;
		    }
		}

        if (!isOnGround)
		{
            isRunning = false;
            if (rb.velocity.y < 0)
			{
                rb.AddForce(Physics.gravity);
                isFalling = true;
			}
		}

        HandleAnimatorStates();
    }

    void HandleAnimatorStates()
	{
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isFalling", isFalling);
    }

	private void OnTriggerEnter(Collider other)
	{
        audioSource.PlayOneShot(stepSound, .5f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Trash"))
		{
            TrashBag trash = collision.gameObject.GetComponent<TrashBag>();
            AddScore(trash.points);
            if (trash.powerUp) { trashSpawner.RushSpawn(); }
            scoreAnimator.SetTrigger("triggerPump");
            audioSource.PlayOneShot(collectSound);
            Destroy(collision.gameObject);
		}

        if (collision.gameObject.GetComponent<Bomb>())
		{
            gameOverText.gameObject.SetActive(true);
            audioSource.PlayOneShot(bombSound);
            StartCoroutine(RestartGame());
            Time.timeScale = 0;
		}
	}

	private void AddScore(int scoreToAdd)
	{
        score += scoreToAdd;
        scoreText.text = score.ToString();
    }

	IEnumerator RestartGame()
	{
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }
}
