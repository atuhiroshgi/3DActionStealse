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
}
