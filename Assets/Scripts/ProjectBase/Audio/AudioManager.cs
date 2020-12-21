using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Audio;


#region
/// <summary>
/// 储存音频信息
/// </summary>
[System.Serializable]
public class Clip
{
    [Header("音频名称")]
    public string clipName;
    [Header("音频片段")]
    public AudioClip audioClip;
    [Header("音量")]
    [Range(0, 1)]
    public float volumn = 1f;
    [Header("音频分类")]
    public AudioMixerGroup audioMixerGroup = AudioManager.Instance.Master;
    [Header("是否循环播放")]
    public bool isLoop = false;
    [Header("是否开局播放")]
    public bool isAwake = false;

    public Clip(AudioClip _clip)
    {
        clipName = _clip.name;
        audioClip = _clip;
    }

    public Clip(AudioClip _clip, AudioMixerGroup _audioMixerGroup)
    {
        clipName = _clip.name;
        audioClip = _clip;
        audioMixerGroup = _audioMixerGroup;
    }
}

#endregion
public class AudioManager : MonoBehaviour
{
    // 单例设置
    #region SINGLETON
    public static AudioManager Instance; // *所有类外调用均可使用这个
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSourceDic = new Dictionary<string, AudioSource>();
            //为list中的clip添加AudioSource组件
            InitializeAudioSource();
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion SINGLETON

    // AudioGroup 设置
    public AudioMixer MainMixer;

    public AudioMixerGroup Master;  // Master 
    public AudioMixerGroup BG;      // Background
    public AudioMixerGroup SFX;     // Sound FX
    public AudioMixerGroup AD;      // Audio
    public AudioMixerGroup MUS;     // Music

    public ReactiveProperty<float> Volumn_Master = new ReactiveProperty<float>();
    [HideInInspector] public ReactiveProperty<float> Volumn_BG = new ReactiveProperty<float>();
    [HideInInspector] public ReactiveProperty<float> Volumn_SFX = new ReactiveProperty<float>();
    [HideInInspector] public ReactiveProperty<float> Volumn_AD = new ReactiveProperty<float>();
    [HideInInspector] public ReactiveProperty<float> Volumn_MUS = new ReactiveProperty<float>();

    // 储存各个group的当前声音值（DB）
    public Dictionary<string, ReactiveProperty<float>> VolumnDic = new Dictionary<string, ReactiveProperty<float>>();

    private string AudioResourcePath; // *Obsolete

   
    [Header("储存音频信息的列表")]
    public List<Clip> clipList = new List<Clip>();
    [Header("每一个音频对应一个播放组件")]
    public Dictionary<string, AudioSource> audioSourceDic = new Dictionary<string, AudioSource>();

    private void Start()
    {
        // 分配AudioGroup
        if (MainMixer != null)
        {
            AudioMixerGroup[] mainGroups = MainMixer.FindMatchingGroups("Master");

            Master = mainGroups[0];

            BG = mainGroups[1];
            SFX = mainGroups[2];
            AD = mainGroups[3];
            MUS = mainGroups[4];
        }

        VolumnDic["Volumn_Master"] = Volumn_Master;

        VolumnDic["Volumn_BG"] = Volumn_BG;
        VolumnDic["Volumn_SFX"] = Volumn_SFX;
        VolumnDic["Volumn_AD"] = Volumn_AD;
        VolumnDic["Volumn_MUS"] = Volumn_MUS;



        // 核心：为所有volumn值订阅更改mixer中对应groupVolumn的handler
        foreach (var item in VolumnDic)
        {
            item.Value
                .Subscribe(_ => UpdateGroupVolumn(item.Key));
        }



    }

    /// <summary>
    /// 初始化所有声音片段的播放
    /// </summary>
    private void InitializeAudioSource()
    {
        foreach (var clip in clipList)
        {
            AudioSource audioSource = AddAudioSource(clip);

            if (clip.isAwake)
            {
                audioSource.Play();
            }
        }
    }

    /// <summary>
    /// 给字典添加AudioSource组件
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    private static AudioSource AddAudioSource(Clip clip)
    {
        GameObject obj = new GameObject(clip.clipName);
        obj.transform.SetParent(AudioManager.Instance.transform);

        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = clip.audioClip;
        audioSource.playOnAwake = clip.isAwake;
        audioSource.loop = clip.isLoop;
        audioSource.volume = clip.volumn;
        audioSource.outputAudioMixerGroup = clip.audioMixerGroup;

        if (Instance.audioSourceDic.ContainsKey(clip.clipName))
        {
            Debug.LogWarning($"名为{clip.clipName}的音频重复，添加失败");
            return null;
        }

        AudioManager.Instance.audioSourceDic.Add(clip.clipName, audioSource);
        return audioSource;
    }

    /// <summary>
    /// 可以在不同场景给播放菜单添加新的音频列表
    /// </summary>
    /// <param name="newClipList"></param>
    public static void AddClipList(List<Clip> newClipList)
    {
        foreach (Clip clip in newClipList)
        {
            AudioSource audioSource = AddAudioSource(clip);
        }
    }

    /// <summary>
    /// 可以在不同场景给播放菜单添加新的音频列表
    /// </summary>
    /// <param name="newClipList"></param>
    public static void RemoveClip(string name)
    {
        if (Instance.audioSourceDic.ContainsKey(name))
        {
            Instance.audioSourceDic.Remove(name);
        }
    }

    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="name"></param>
    public static void PlayClip(string name)
    {
        if (!Instance.audioSourceDic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }
        Debug.Log(name + "播放");
        Instance.audioSourceDic[name].Play();
    }

    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="name"></param>
    public static void PlayClip(string name,bool isWait)
    {
        if (!Instance.audioSourceDic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }
        if (isWait)
        {
            if (!Instance.audioSourceDic[name].isPlaying)
            {
                Debug.Log(name + "播放");
                Instance.audioSourceDic[name].Play();
            }
        }
        else
        {
            Debug.Log(name + "播放");
            Instance.audioSourceDic[name].Play();
        }
        
    }

    /// <summary>
    /// 按秒数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="time">延时多少秒关闭</param>
    public static void PlayClip(string name,float time)
    {
        Dictionary<string, AudioSource> dic = Instance.audioSourceDic;

        if (!dic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }

        GameObject audioController = GlobalTool.CreatEmptyObj(Instance.gameObject, "audioController");

        dic[name].Play();
        Observable.Timer(System.TimeSpan.FromSeconds(time))
            .Subscribe(_ =>
            {
                if (dic[name].isPlaying)
                {
                    StopClip(name);
                }
                if (audioController)
                {
                    Destroy(audioController);
                }
            })
            .AddTo(audioController);
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    /// <param name="name"></param>
    public static void StopClip(string name)
    {
        if (!Instance.audioSourceDic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }

        Instance.audioSourceDic[name].Stop();
    }

    /// <summary>
    /// 停止播放并淡出
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fadeTime"></param>
    public static void StopClip(string name,float fadeTime)
    {
        if (!Instance.audioSourceDic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }
        if (!Instance.audioSourceDic[name].isPlaying)
        {
            Debug.LogWarning($"音频{name}未在播放");
            return;
        }

        GameObject fadeController = GlobalTool.CreatEmptyObj(Instance.gameObject, "fadeController");
        AudioSource audioSource = Instance.audioSourceDic[name];
        float recordVolumn = audioSource.volume;
        float velocity = 0f;

        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                if (audioSource.volume > 0.01f)
                {
                    audioSource.volume = Mathf.SmoothDamp(audioSource.volume, 0, ref velocity, fadeTime);
                }
                else
                {
                    audioSource.volume = 0;
                    audioSource.Stop();
                    //还原
                    audioSource.volume = recordVolumn;
                    if (fadeController)
                    {
                        Destroy(fadeController);
                    }
                }
            })
            .AddTo(fadeController);
    }

    /// <summary>
    /// 缓慢调整音量
    /// </summary>
    /// <param name="name"></param>
    public static void FadeClipFrom(string name, float target, float fadeTime)
    {
        if (!Instance.audioSourceDic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }
        if (Instance.audioSourceDic[name].isPlaying)
        {
            Debug.LogWarning($"音频正在播放");
            return;
        }


        GameObject fadeController = GlobalTool.CreatEmptyObj(Instance.gameObject);
        AudioSource audioSource = Instance.audioSourceDic[name];
        float velocity = 0f;
        float recordVolumn = audioSource.volume;
        audioSource.volume = target;

        if (!Instance.audioSourceDic[name].isPlaying)
        {
            audioSource.Play();
        }

        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                if (Mathf.Abs(audioSource.volume - recordVolumn) > 0.01f)
                {
                    audioSource.volume = Mathf.SmoothDamp(audioSource.volume, recordVolumn, ref velocity, fadeTime);
                }
                else
                {
                    audioSource.volume = recordVolumn;
                    if (fadeController)
                    {
                        Destroy(fadeController);
                    }
                }
            })
            .AddTo(fadeController);

    }

    /// <summary>
    /// 缓慢调整音量
    /// </summary>
    /// <param name="name"></param>
    public static void FadeClipTo(string name,float target,float fadeTime)
    {
        if (!Instance.audioSourceDic.ContainsKey(name))
        {
            Debug.LogWarning($"音频{name}不存在");
            return;
        }

        GameObject fadeController = GlobalTool.CreatEmptyObj(Instance.gameObject);
        AudioSource audioSource = Instance.audioSourceDic[name];
        float velocity = 0f;

        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                if (Mathf.Abs(audioSource.volume-target) > 0.01f)
                {
                    audioSource.volume = Mathf.SmoothDamp(audioSource.volume, target, ref velocity, fadeTime);
                }
                else
                {
                    audioSource.volume = target;
                    if (fadeController)
                    {
                        Destroy(fadeController);
                    }
                }
            })
            .AddTo(fadeController);
        
    }


    // 主动更改volumn时的handler
    public void SetGroupVolumn(string _volumnName, float _value)
    {
        if (VolumnDic.ContainsKey(_volumnName))
        {
            VolumnDic[_volumnName].Value = SliderToDB(_value);
        } else
        {
            Debug.Log("Can not found Audio Group: " + _volumnName);
        }
    }
   

    // volumn改变时的handler
    public void UpdateGroupVolumn(string _volumnName)
    {
        if (VolumnDic.ContainsKey(_volumnName))
        {
            MainMixer.SetFloat(_volumnName, VolumnDic[_volumnName].Value);
        }
        else
        {
            Debug.Log("Can not found Audio Group: " + _volumnName);
        }
    }

    // 工具
    // 转化0-1的单位（slider）到真实声音单位（DB，以-80为最低值）
    public static float SliderToDB(float _x)
    {
        return (_x * 80f - 80f);
    }

    public static float DBToSlider(float _y)
    {
        return ((_y + 80f) / 80f);
    }

}
