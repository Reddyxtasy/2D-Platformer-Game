using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource sfxAudio;
    public AudioClip buttonClickSound;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonClick()
    {
        if (sfxAudio != null && buttonClickSound != null)
        {
            sfxAudio.PlayOneShot(buttonClickSound);
        }
    }
}
