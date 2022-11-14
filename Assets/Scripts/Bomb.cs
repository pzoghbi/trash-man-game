using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bomb : MonoBehaviour
{
    [SerializeField] AudioClip bounceSound;
    [SerializeField] ParticleSystem bounceParticles;

    Animator animator;
    AudioSource audioSource;
    MeshRenderer[] meshRenderers;
    bool flash = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        Destroy(gameObject, 10);
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
            animator.SetTrigger("triggerSquash");
            audioSource.PlayOneShot(bounceSound);
            ParticleSystem particles = Instantiate(bounceParticles, collision.GetContact(0).point, Quaternion.identity);
            Destroy(particles.gameObject, particles.main.duration);
		}
	}
}
