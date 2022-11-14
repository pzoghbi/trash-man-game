using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TrashSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] spawnableObjects;
    [SerializeField] float forceAmount = 20;
    [SerializeField] AudioClip spawnSound;

    AudioSource audioSource;

    public bool rush = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnTrash());
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RushSpawn()
	{
        StartCoroutine(Rush());
	}

    IEnumerator Rush()
	{
        rush = true;
        yield return new WaitForSeconds(2);
        rush = false;
	}

    IEnumerator SpawnTrash()
	{
        float waitForSecs = rush ? 0.1f : Random.Range(1.23f, 2.23f);
        yield return new WaitForSecondsRealtime(waitForSecs);

        int spawnObjectIndex = rush ? 0 
            : (Random.Range(0, 100) < 80) ? 0 
            : (Random.Range(0, 100) < 80) ? 1 
            : 2;
        
        GameObject trashBag = Instantiate(spawnableObjects[spawnObjectIndex], transform.position, Quaternion.identity);
        forceAmount = Random.Range(20, 50);
        trashBag.GetComponent<Rigidbody>().AddForce(forceAmount * (Vector3.up + Vector3.forward), ForceMode.Impulse);

        audioSource.PlayOneShot(spawnSound);
        StartCoroutine(SpawnTrash());
	}
}
