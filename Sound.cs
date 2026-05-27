using UnityEngine;

public class Sound : MonoBehaviour
{
    public static Sound Instance { get; private set; }

    private AudioSource AudioSource;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            AudioSource = GetComponent<AudioSource>();

            int isMuted = PlayerPrefs.GetInt("MusicMuted", 0);
            AudioSource.mute = (isMuted == 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool ToggleMusic()
    {
        AudioSource.mute = !AudioSource.mute;

        PlayerPrefs.SetInt("MusicMuted", AudioSource.mute ? 1 : 0);
        PlayerPrefs.Save();

        return AudioSource.mute; 
    }

    public bool IsMuted()
    {
        return AudioSource.mute;
    }
}
