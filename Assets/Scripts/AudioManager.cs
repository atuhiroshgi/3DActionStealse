using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
    }

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
}
