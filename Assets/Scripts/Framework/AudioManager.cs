using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;

    public static AudioManager getInstance()
    {
        if (_instance == null)
        {
            Debug.LogWarning("AudioManager is null");
        }
        return _instance;
    }

    public AudioSource audioSource;
    public string audioName;

    void Awake()
    {
        _instance = this;

        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        if (audioName != null)
        {
            PlayMusic(audioName);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void PlaySound(string name)
    {
        AudioClip clip = ResourceManager.LoadResource<AudioClip>("Audio", name);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayMusic(string name, bool loop = true)
    {
        AudioClip clip = ResourceManager.LoadResource<AudioClip>("Audio", name);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.loop = loop;
            audioSource.Play();
        }
    }

    public void StopMusic(string name)
    {
        audioSource.Stop();
    }
}
