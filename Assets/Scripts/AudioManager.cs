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
    private void Start()
    {
        bgmSource.volume = GameManager.Instance.GetVolume();
        sfxSource.volume = GameManager.Instance.GetVolume();
    }

    /// <summary>
    /// シングルトンの実装
    /// </summary>
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            //シーンがロードされたときにOnSceneLoadedを実行
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// このスクリプトがアタッチされたオブジェクトが破壊されたら実行される
    /// </summary>
    private void OnDestroy()
    {
        if(Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// シーンが読み込まれた時に実行されるメソッド
    /// </summary>
    private void OnEnable()
    {
        //Volumeが変わったらSetVolumeが実行される
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnVolumeChanged += SetVolume;
        }
    }

    /// <summary>
    /// シーンが切り替わるときに実行されるメソッド
    /// </summary>
    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnVolumeChanged -= SetVolume;
        }
    }

    /// <summary>
    /// BGMを流すときに実行するメソッド
    /// </summary>
    /// <param name="bgmName">BGMの名前</param>
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

        Debug.Log(bgmName);
    }
    
    /// <summary>
    /// BGMを止めるときに実行するメソッド
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// 効果音を流すときに実行するメソッド
    /// </summary>
    /// <param name="sfxName">効果音の名前</param>
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

    /// <summary>
    /// 音量をセットするときに実行するメソッド
    /// </summary>
    /// <param name="volume">音量</param>
    public void SetVolume(float volume)
    {
        if(bgmSource != null) bgmSource.volume = volume * 0.7f;
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
