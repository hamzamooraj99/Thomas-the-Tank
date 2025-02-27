using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundObject;

    void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PlaySoundFXClip(AudioClip clip, Transform spawn, float volume)
    {
        //Spawn GameObject
        AudioSource audioSource = Instantiate(soundObject, spawn.position, Quaternion.identity);

        //Assign AudioClip
        audioSource.clip = clip;

        //Assign Volume
        audioSource.volume = volume;

        //Play Sound
        audioSource.Play();

        //Get Length of Sound Clip
        float clipLength = audioSource.clip.length;

        //Destroy clip after it is done
        Destroy(audioSource.gameObject, clipLength);
    }
}
