using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using RotaryHeart.Lib.SerializableDictionary;
using BasicTools.ButtonInspector;

[System.Serializable]
public class MusicSec
{
    // Default fixed tempo
    public string name;
    public AudioClip Clip;
    [System.NonSerialized] public int barCount; // length

    [Range(0,1)] public float volumn = 1;

    [System.NonSerialized] public AudioSource Source;
}


[System.Serializable]
public class MusicSecList
{
    public List<MusicSec> musicSecList = new List<MusicSec>();
}


public class MusicManager : MonoSingletonCO<MusicManager>
{

    // unified music property
    public int bar;
    public int beat;
    public float tempo;

    [SerializeField] private int trackCount;

    // Current Available Sections, Update every bar
    [SerializeField] List<MusicSec> MusicSections = new List<MusicSec>();

    [System.Serializable]
    public class MusicStateDictionary : SerializableDictionaryBase<string, MusicSecList> { }
    [SerializeField] MusicStateDictionary MusicStateDic = new MusicStateDictionary();

    private void Start()
    {
        
    }

    private void InitializeMusicSections()
    {
        foreach(var sec in MusicSections)
        {

            // refull audiosource
            if(sec.Source == null)
            {
                if(sec.name == null || sec.Clip == null)
                {
                    Debug.LogError("Can not Initialize mus sec without clip/name/volumn ");
                    return;
                } else
                {
                    AddAudioSource(sec.Clip, true, sec.volumn);
                
                }
            }
        }
    }

    private AudioSource AddAudioSource(AudioClip _clip, bool _isLoop, float _volumn)
    {
        var source = gameObject.AddComponent<AudioSource>();

        source.clip = _clip;
        source.loop = _isLoop;
        source.volume = _volumn;

        source.outputAudioMixerGroup = AudioManager.Instance.MUS;

        return source;
    }

   
    private MusicSec CreateMusicSection(string _secName, AudioClip _clip)
    {
        MusicSec ms = new MusicSec();

        // Music Section Settings
        ms.name = _secName;
        ms.Clip = _clip;
        ms.Source = AddAudioSource(_clip, true, 1);

        return ms;
    }

    private void AddMusicSection(MusicSec _sec)
    {
        MusicSections.Add(_sec);
    }

    
    private void AddMusicSection(string _secName, AudioClip _clip)
    {
        MusicSections.Add(CreateMusicSection(_secName, _clip));
    }

    private void RemoveMusicSection(string _secName)
    {
        foreach(var sec in MusicSections)
        {
            if(sec.name == _secName)
            {
                Debug.Log("Remove Section: " + sec.name);
                Destroy(sec.Source); // remove from manager
                MusicSections.Remove(sec);
            }
        }
    }

    
    
}



