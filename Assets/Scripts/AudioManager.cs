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
        //bgmClipsÇÃíÜÇ©ÇÁbgmNameÇ∆àÍívÇ∑ÇÈñºëOÇíTÇµÇƒclipÇ…äiî[
        AudioClip clip = bgmClips.Find(bgm => bgm.name == bgmName);
        if(clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"å¯â âπÉNÉäÉbÉvÇ™ë∂ç›ÇµÇ‹ÇπÇÒ:{bgmName}");
        }

        Debug.Log(bgmName);
    }

    public void PlaySFX(string sfxName)
    {
        //sfxClipsÇÃíÜÇ©ÇÁsfxNameÇ∆àÍívÇ∑ÇÈñºëOÇíTÇµÇƒclipÇ…äiî[
        AudioClip clip = sfxClips.Find(sfx => sfx.name == sfxName);
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"å¯â âπÉNÉäÉbÉvÇ™ë∂ç›ÇµÇ‹ÇπÇÒ:{sfxName}");
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void SetVolume(float volume)
    {
        if(bgmSource != null) bgmSource.volume = volume - 0.3f;
        if(sfxSource != null) sfxSource.volume = volume;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bgmSource.Stop();
        bgmSource.clip = null;
        sfxSource.Stop();
        sfxSource.Stop();
    }
}
