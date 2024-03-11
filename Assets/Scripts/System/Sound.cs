using System.Collections;
using UnityEngine;

public class Sound : MonoBehaviour, OnReturnPool<Sound>, SoundManager.OnEndBGM
{
    OnReturnPoolEvent<Sound> OnReturnPool;
    SoundManager.OnEndBGMEvent OnEndBGMEvent;

    private AudioSource audioSource;

    private void OnEnable()
    {
        SetEndBGMEvent(SoundManager.Instance.EndBGM);
    }

    public void Play(AudioClip clip, float vol)
    {
        audioSource.clip = clip;
        audioSource.volume = vol;
        audioSource.Play();

        StartCoroutine(Playing());
    }

    private IEnumerator Playing()
    {
        string[] names = audioSource.clip.name.Split("_");

        if (names[0] == "BGM")
        {
            OnEndBGMEvent(audioSource);
        }

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        OnReturnPool?.Invoke(this);
    }

    public void OnInteraction()
    {
        OnReturnPool(this);
    }

    public void Init(OnReturnPoolEvent<Sound> OnReturnPool)
    {
        this.OnReturnPool = OnReturnPool;
        
        audioSource = GetComponent<AudioSource>();
    }

    public void SetEndBGMEvent(SoundManager.OnEndBGMEvent OnEndBGMEvent)
    {
        this.OnEndBGMEvent = OnEndBGMEvent;
    }
}
