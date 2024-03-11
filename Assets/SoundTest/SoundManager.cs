using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : ObjectPooling<SoundManager, Sound>
{
    public interface OnEndBGM
    {
        public void SetEndBGMEvent(OnEndBGMEvent OnEndBGMEvent);
    }

    public delegate void OnEndBGMEvent(AudioSource audioSource);

    public enum SoundType
    {
        Master,
        Bgm,
        Effect,
    }

    public float masterVol { get; private set; }
    public float bgmVol { get; private set; }
    public float effectVol { get; private set;}

    private const string KEY_MASTER = "MasterVol";
    private const string KEY_BGM = "BgmVol";
    private const string KEY_EFFECT = "EffectVol";

    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] effects;

    [Header("Source")]
    [SerializeField] private AudioSource nowPlaySource;

    private new void Awake()
    {
        base.Awake();

        nowPlaySource = gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //TestGetSound(SoundType.Effect);

        //bgmSource.clip = Resources.Load("BGM_Sukidakara",typeof(AudioClip)) as AudioClip;

        //encrypt넣어서 할 듯
        masterVol = PlayerPrefs.HasKey(KEY_MASTER) ? PlayerPrefs.GetFloat(KEY_MASTER) : 1.0f;
        bgmVol = PlayerPrefs.HasKey(KEY_BGM) ? PlayerPrefs.GetFloat(KEY_BGM) : 1.0f;
        effectVol = PlayerPrefs.HasKey(KEY_EFFECT) ? PlayerPrefs.GetFloat(KEY_EFFECT) : 1.0f;

        //bgmSource.volume = bgmVol * masterVol;

        //bgmSource.Play();
    }

    public void OnChangedVol(SoundType type, float value)
    {
        switch (type)
        {
            case SoundType.Master:
            masterVol = value;
            PlayerPrefs.SetFloat(KEY_MASTER, value);
            break;
            case SoundType.Bgm:
            bgmVol = value;
            PlayerPrefs.SetFloat(KEY_BGM, value);
            break;
            case SoundType.Effect:
            effectVol = value;
            PlayerPrefs.SetFloat(KEY_EFFECT, value);
            break;
        }

        nowPlaySource.volume = masterVol * bgmVol;
    }

    public AudioClip GetClip(SoundType type, string name)
    {
        AudioClip[] clips = (type == SoundType.Bgm) ? bgms : effects;

        foreach (AudioClip clip in clips)
        {
            if (clip.name == name)
            {
                if (type == SoundType.Bgm)
                {
                    PauseBGM();
                    nowPlaySource.clip = null;
                }

                return clip;
            }
        }

        return null;
    }

    public void PlayBGM(string name)
    {
        nowPlaySource.clip = GetClip(SoundType.Bgm, name);

        nowPlaySource.Play();
    }

    public void PauseBGM()
    {
        nowPlaySource.Pause();
    }

    public void UnPauseBGM()
    {
        nowPlaySource.UnPause();
    }

    public void PlaySound(string name)
    {
        Sound sound = GetPool();
        string[] soundType = name.Split('_');

        sound.Play(GetClip(soundType[0] == "Effect" ? SoundType.Effect : SoundType.Bgm, name), masterVol * effectVol);
    }

    public void EndBGM(AudioSource audioSource)
    {
        nowPlaySource = audioSource;
    }

    public string[] TestGetSound(SoundType type)
    {
        string[] audioClips;
        int count = type == SoundType.Effect ? effects.Length : bgms.Length;

        audioClips = type == SoundType.Effect ? new string[effects.Length] : new string[bgms.Length];

        for (int i = 0; i < count; i++)
        {
            audioClips[i] = type == SoundType.Effect ? effects[i].name : bgms[i].name;
        }

        return audioClips;
    }
}
