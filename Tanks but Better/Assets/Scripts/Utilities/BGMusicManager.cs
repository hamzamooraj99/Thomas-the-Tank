using UnityEngine;

public class BGMusicManager : MonoBehaviour
{
    public static BGMusicManager instance;
    
    [Header("Music")]
    [SerializeField] public AudioClip inGameMusic;
    [SerializeField] public AudioClip loseMusic;
    [SerializeField] public AudioClip winMusic;
    private AudioSource audioSource;

    void Start()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
        
        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = inGameMusic;
        audioSource.loop = true;
        audioSource.volume = 0.2f;
        audioSource.Play();
    }

    public void StopTrack()
    {
        audioSource.Stop();
    }

    public void ChangeToLose()
    {
        audioSource.Stop();
        audioSource.clip = loseMusic;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }

    public void ChangeToWin()
    {
        audioSource.Stop();
        audioSource.clip = winMusic;
        audioSource.volume = 0.5f;
        audioSource.Play();
    }
}
