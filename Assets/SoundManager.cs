using System.Collections.Generic;
using UnityEngine;


public class SoundManager : ObjectPooling<SoundManager, Sound>
{
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
    private const string KEY_EFECT = "EffectVol";

    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] effects;

    [Header("Source")]
    [SerializeField] private AudioSource bgmSource;

    private new void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        //
        masterVol = PlayerPrefs.HasKey(KEY_MASTER) ? PlayerPrefs.GetFloat(KEY_MASTER) : 1.0f;
    }
}
