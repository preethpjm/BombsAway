using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Explosion Sounds")]
    public AudioClip explosion1;
    public AudioClip explosion2;
    public AudioClip explosion3;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayExplosion1()
    {
        PlaySound(explosion1);
    }
    public void PlayExplosion2()
    {
        PlaySound(explosion2);
    }
    public void PlayExplosion3()
    {
        PlaySound(explosion3);
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioClip is null");
        }
    }
}