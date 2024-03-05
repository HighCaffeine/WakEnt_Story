using System.Collections;
using UnityEngine;

public class Sound : MonoBehaviour, OnReturnPool<Sound>
{
    OnReturnPoolEvent<Sound> OnReturnPool;

    private AudioSource audioSource;

    public void Play(AudioClip clip, float vol)
    {
        audioSource.clip = clip;
        audioSource.volume = vol;
        audioSource.Play();

        StartCoroutine(Playing());
    }

    private IEnumerator Playing()
    {
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
}
