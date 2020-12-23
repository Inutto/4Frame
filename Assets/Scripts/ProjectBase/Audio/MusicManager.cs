using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

using RotaryHeart.Lib.SerializableDictionary;
using BasicTools.ButtonInspector;
using System;

[System.Serializable]
public class MusicSec
{
    // Default fixed tempo
    public AudioClip Clip;
    [Range(0, 1)] public float volumn = 1;

    public int barCount; // length
    [System.NonSerialized] public AudioSource Source;


    public void FadeVolumnTo(float _volumn, float _time)
    {
        Debug.Log("Fade volumn to " + _volumn);
        LeanTween.value(this.Source.gameObject, FadeVolumnCallback, this.Source.volume, _volumn, _time);

    }

    public void FadeVolumnCallback(float _volumn)
    {
        this.Source.volume = _volumn;
    }

    public void Play()
    {
        this.Source.loop = true;
        this.Source.Play();
    }

    public void PauseOnLoopFinish()
    {
        this.Source.loop = false;
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



// TEMP for listen
public class EventListener<T>
{
    public delegate void OnValueChangeDelegate(T newVal);
    public event OnValueChangeDelegate OnVariableChange;
    private T m_value;
    public T Value
    {
        get
        {
            return m_value;
        }
        set
        {
            if (m_value.Equals(value)) return;
            OnVariableChange?.Invoke(value);
            m_value = value;
        }
    }
}





public class MusicManager : MonoSingletonCO<MusicManager>
{

    [Header("Unified Musical Property")]
    public float secondsPerBar;

    [Header("Section List")]
    // Current Available Sections, Update every bar
    [SerializeField] MusicSectionDictionary MusicSections = new MusicSectionDictionary();

    [Header("Section State Dictionary")]
    public string _currentMusicState;
    [SerializeField] MusicStateDictionary MusicStateDic = new MusicStateDictionary();

    

    private void Start()
    {
        InitializeMusicSections();

        StartMusicState("Test");

        //bservable.Timer(TimeSpan.FromSeconds(3f))
        //    .Subscribe(_ => FadeToMusicState("Test", "Test2"));

        MusicSections["Harp1"].FadeVolumnTo(0.2f, 4f);

    }

    private void FadeToMusicState(string _currentMusicState, string _state)
    {
        Debug.Log("Fade");
        if (!MusicStateDic.ContainsKey(_state))
        {
            Debug.LogWarning("state -> " + _state + " can not be found in current music state dic. no change made.");
            return;
        }

        if (_currentMusicState == null) StartMusicState(_state);


        // Trans
        var currentSecNameList = MusicStateDic[_currentMusicState];
        var targetSecNameList = MusicStateDic[_state];

        MusicSec lastSec = new MusicSec();
        foreach(var name in currentSecNameList.musicSecNameList)
        {
            MusicSections[name].PauseOnLoopFinish();
            Debug.Log("Sec -> " + name + " pause on next loop");
            lastSec = MusicSections[name];
        }

        if(lastSec.Source == null)
        {
            Debug.LogError("The sample sec's source is empty");
        }

        Observable.EveryUpdate()
            .Where(_ => lastSec.Source.isPlaying == false)
            .First()
            .Subscribe(_ =>
            {
                Debug.Log("last sec stop playing");
                foreach (var name in targetSecNameList.musicSecNameList)
                {
                    var sec = MusicSections[name];
                    sec.Play();
                }

            }
            );
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


    private void SetSectionsLoop(string _musicState, bool _state)
    {
        if (!MusicStateDic.ContainsKey(_musicState))
        {
            Debug.LogWarning("State -> " + _musicState + " can not be found in current music state dic");
            return;
        }

        foreach(var name in MusicStateDic[_musicState].musicSecNameList)
        {
            var sec = MusicSections[name];
            sec.Source.loop = _state;
        }
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



