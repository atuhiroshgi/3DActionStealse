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
    /// �V���O���g���̎���
    /// </summary>
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            //�V�[�������[�h���ꂽ�Ƃ���OnSceneLoaded�����s
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���̃X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g���j�󂳂ꂽ����s�����
    /// </summary>
    private void OnDestroy()
    {
        if(Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// �V�[�����ǂݍ��܂ꂽ���Ɏ��s����郁�\�b�h
    /// </summary>
    private void OnEnable()
    {
        //Volume���ς������SetVolume�����s�����
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnVolumeChanged += SetVolume;
        }
    }

    /// <summary>
    /// �V�[�����؂�ւ��Ƃ��Ɏ��s����郁�\�b�h
    /// </summary>
    private void OnDisable()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.OnVolumeChanged -= SetVolume;
        }
    }

    /// <summary>
    /// BGM�𗬂��Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    /// <param name="bgmName">BGM�̖��O</param>
    public void PlayBGM(string bgmName)
    {
        //bgmClips�̒�����bgmName�ƈ�v���閼�O��T����clip�Ɋi�[
        AudioClip clip = bgmClips.Find(bgm => bgm.name == bgmName);
        if(clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"���ʉ��N���b�v�����݂��܂���:{bgmName}");
        }

        Debug.Log(bgmName);
    }
    
    /// <summary>
    /// BGM���~�߂�Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    /// <summary>
    /// ���ʉ��𗬂��Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    /// <param name="sfxName">���ʉ��̖��O</param>
    public void PlaySFX(string sfxName)
    {
        //sfxClips�̒�����sfxName�ƈ�v���閼�O��T����clip�Ɋi�[
        AudioClip clip = sfxClips.Find(sfx => sfx.name == sfxName);
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"���ʉ��N���b�v�����݂��܂���:{sfxName}");
        }
    }

    /// <summary>
    /// ���ʂ��Z�b�g����Ƃ��Ɏ��s���郁�\�b�h
    /// </summary>
    /// <param name="volume">����</param>
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
