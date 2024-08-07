using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private List<AudioClip> bgmClips;
    [SerializeField] private List<AudioClip> sfxClips;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        bgmSource.volume = GameManager.Instance.GetVolume();
        sfxSource.volume = GameManager.Instance.GetVolume();
    }

    private void OnEnable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnVolumeChanged += SetVolume;
        }
    }
    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnVolumeChanged -= SetVolume;
        }
    }

    public void PlayBGM(string bgmName)
    {
        //bgmClipsの中からbgmNameと一致する名前を探してclipに格納
        AudioClip clip = bgmClips.Find(bgm => bgm.name == bgmName);
        if(clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"効果音クリップが存在しません:{bgmName}");
        }
    }

    public void PlaySFX(string sfxName)
    {
        //sfxClipsの中からsfxNameと一致する名前を探してclipに格納
        AudioClip clip = sfxClips.Find(sfx => sfx.name == sfxName);
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"効果音クリップが存在しません:{sfxName}");
        }
    }

    public void SetVolume(float volume)
    {
        if(bgmSource != null) bgmSource.volume = volume;
        if(sfxSource != null) sfxSource.volume = volume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }
}
