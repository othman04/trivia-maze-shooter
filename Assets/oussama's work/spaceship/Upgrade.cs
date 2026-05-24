using System.Collections;
using UnityEngine;

public class UpgradeTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip doorSlideSound;
    public AudioClip upgradeSound;

    private bool hasPlayed = false;

    void Start()
    {
        audioSource.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            hasPlayed = true;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        audioSource.clip = doorSlideSound;
        audioSource.Play();
        yield return new WaitForSeconds(doorSlideSound.length);
        audioSource.clip = upgradeSound;
        audioSource.Play();
    }
}