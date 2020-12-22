using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class MusicManager : MonoSingletonGO<MusicManager>
{
    // All the Available Sections
    Dictionary<string, MusicSec> musicDic = new Dictionary<string, MusicSec>();
       
    // Current Available Sections, Update every bar
    List<MusicSec> currentSections = new List<MusicSec>();


    [SerializeField] private int trackCount;


    private void Start()
    {
        trackCount = currentSections.Count;
    }


    void AddSections(MusicSec _newSec)
    {
        currentSections.Add(_newSec);
    }


    
}


public class MusicSec
{
    // Default fixed tempo
    private AudioSource _Source;
    public AudioClip _clip;
    public int barCount;
}
