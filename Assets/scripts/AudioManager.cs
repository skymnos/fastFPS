using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;

    [Header("Music")]
    [SerializeField] private AudioClip[] music;
    private int musicIndex;

    private void Start()
    {
        musicIndex = 0;
        musicSource.clip = music[musicIndex];
        musicSource.Play();
        Invoke("nextMusic", musicSource.clip.length);
    }

    private void nextMusic()
    {
        musicIndex++;
        if (musicIndex > music.Length) musicIndex = 0;

        musicSource.clip = music[musicIndex];
        musicSource.Play();
        Invoke("nextMusic", musicSource.clip.length);
    }
}
