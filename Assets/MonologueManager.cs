using UnityEngine;

public class MonologueManager : MonoBehaviour
{
    [System.Serializable]
    public class MonologueZone
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [HideInInspector] public bool hasPlayed = false;
    }

    public MonologueZone[] monologues;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void TriggerMonologue(string monologueName)
    {
        foreach (MonologueZone m in monologues)
        {
            if (m.name == monologueName && !m.hasPlayed && m.clip != null)
            {
                m.hasPlayed = true;
                audioSource.volume = m.volume;
                audioSource.clip = m.clip;
                audioSource.Play();
                break;
            }
        }
    }
}