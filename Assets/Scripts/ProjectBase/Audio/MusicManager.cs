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
    public AudioClip Clip;
    [Range(0, 1)] public float volumn = 1;
    [SerializeField] ReactiveProperty<float> volumn_control = new ReactiveProperty<float>();

    [System.NonSerialized] public int barCount; // length
    [System.NonSerialized] public AudioSource Source;

    public void InitializeVolumnSubs()
    {
        volumn_control
            .Subscribe(value =>
            {
                Debug.Log("Subs for volumn_control");
                if (Source) Source.volume = value;
                volumn = value;
            }
            ).AddTo(this.Source);
    }
}

/// <summary>
/// {SecName : Sec} Store All Music Sections by Name (Key)
/// </summary>
[System.Serializable]
public class MusicSectionDictionary : SerializableDictionaryBase<string, MusicSec> { }

/// <summary>
/// {StateName : SecNameList} Store All Music State by stateName (Key)
/// </summary>
[System.Serializable]
public class MusicStateDictionary : SerializableDictionaryBase<string, MusicSecNameList> { }

[System.Serializable]
public class MusicSecNameList
{
    public List<string> musicSecNameList = new List<string>();
}




public class MusicManager : MonoSingletonCO<MusicManager>
{


   
    [Header("Unified Musical Property")]
    public int bar;
    public int beat;
    public float tempo;
    [SerializeField] private int trackCount;

    [Header("Section List")]
    // Current Available Sections, Update every bar
    [SerializeField] MusicSectionDictionary MusicSections = new MusicSectionDictionary();

    [Header("Section State Dictionary")]
    [SerializeField] MusicStateDictionary MusicStateDic = new MusicStateDictionary();

    private void Start()
    {
        InitializeMusicSections();

        StartMusicState("Test");


    }

    public void TestChangeState()
    {
        StartMusicState("Test");
    }

    private void InitializeMusicSections()
    {
        foreach(var item in MusicSections)
        {
            var sec = item.Value;
            // refull audiosource

            if(sec.Source == null)
            {
                if(item.Key == null || sec.Clip == null)
                {
                    Debug.LogError("Can not Initialize mus sec without clip/name/volumn ");
                    return;
                } else
                {
                    var source = AddAudioSource(sec.Clip, true, sec.volumn);
                    sec.Source = source;
                
                }
            }

            sec.InitializeVolumnSubs();
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

   
    private MusicSec CreateMusicSection(AudioClip _clip)
    {
        MusicSec ms = new MusicSec();

        // Music Section Settings
        ms.Clip = _clip;
        ms.Source = AddAudioSource(_clip, true, 1);

        return ms;
    }

    private void AddMusicSection(string _secName, MusicSec _sec)
    {
        MusicSections.Add(_secName, _sec);
    }

    
    private void AddMusicSection(string _secName, AudioClip _clip)
    {
        var sec = CreateMusicSection(_clip);
        MusicSections.Add(_secName, sec);
    }

    private void RemoveMusicSection(string _secName)
    {
        if (!MusicSections.ContainsKey(_secName))
        {
            Debug.LogWarning("Trying to remove music section -> " + _secName + " which not exists in musicsections");
            return;
        }

        MusicSections.Remove(_secName);
    }

    
    public void StartMusicState(string _stateName)
    {
        if (!MusicStateDic.ContainsKey(_stateName))
        {
            Debug.LogError("Can not find Music State with name ->" + _stateName);
            return;
        }

        var nameList = MusicStateDic[_stateName].musicSecNameList;

        foreach(var name in nameList)
        {
            Debug.Log("Current name: " + name);
            MusicSections[name].Source.Play();
        }
    }

    
    
}



