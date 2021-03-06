using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using RotaryHeart.Lib.SerializableDictionary;
using BasicTools.ButtonInspector;

namespace FourFrame.TopDown
{


    public class Timeline : MonoBehaviour
    {
        [Header("Timeline Settings")]
        [SerializeField] private TimelineManager timelineManager;
        [SerializeField] private bool canCreateTimeline = true;
        /// <summary>
        /// Reference of timelineManager.currentTick
        /// </summary>
        public int currentTick
        {
            get
            {
                return timelineManager.currentTick;
            }

            set
            {
                timelineManager.currentTick = value;
            }
        }

        [SerializeField] private bool isPlaying;

        [Button("Play", "PlayTest")]
        public bool button_1;

        [Button("Undo", "Undo")]
        public bool button_2;

        /// <summary>
        /// Analyze BaseInfo Type and return this enum to pass to different minor handler
        /// </summary>
        enum COMMAND_HANDLER_CASE
        {
            PLAYER_MOVE,
            PORTAL_RESET_POSITION,
            ITEM_TRIGGER,
            PORTAL_TRIGGER,
            NULL
        }

        [Space(2)]
        [Header("Attributes")]
        public TickInfoDic tickInfoDic = new TickInfoDic();


        private void Start()
        {
            // TODO: Replace with normal get method
            timelineManager = GameObject.Find("TimelineManager")
                .GetComponent<TimelineManager>();


            InitEmptyTickInfoDic(currentTick);
        }

        #region INIT


        public void SubscribeInstances()
        {
            foreach(var instance in timelineManager.instancesList)
            {
                instance.OnCommand += InstanceCommandHandler;
            }
        }

        public void UnSubscribeInstances()
        {
            foreach(var instance in timelineManager.instancesList)
            {
                instance.OnCommand -= InstanceCommandHandler;
            }
        }

        public void SubscribeInstance(Instance instance)
        {
            instance.OnCommand += InstanceCommandHandler;
        }

        public void UnSubscribeInstance(Instance instance)
        {
            instance.OnCommand -= InstanceCommandHandler;
        }



        



        #endregion


        #region TICKINFODIC


        /// <summary>
        /// Init current tick (new TickInfo)
        /// </summary>
        /// <param name="tick"></param>
        public void InitEmptyTickInfoDic(int tick)
        {
            tickInfoDic[tick] = new TickInfo();
        }

        private void Record(BaseInfo _info)
        {
            var currentTickInfo = tickInfoDic[currentTick];
            currentTickInfo.Add(_info);
            if(_info.isTickEnd == true)
            {
                timelineManager.GotoNextTick();
            }
        }


        public void Undo()
        {
            var lastTick = currentTick - 1;

            if (lastTick > 0 && !isPlaying)
            {
                tickInfoDic.Remove(currentTick);
                timelineManager.GotoPreviousTick();
                InitEmptyTickInfoDic(currentTick);
            }
            else
            {
                Debug.LogWarning("Can not undo now");
            }
        }


        #endregion


        #region EVENT HANDLER


        /// <summary>
        /// Discuss the type of {baseInfo} and {instance} to decide which handler to use
        /// </summary>
        /// <param name="baseInfo"></param>
        private void InstanceCommandHandler(BaseInfo baseInfo)
        {
            var case_instance = BaseInfo2Case(baseInfo);
            switch (case_instance)
            {
                case COMMAND_HANDLER_CASE.ITEM_TRIGGER:
                    ItemTriggerHandler(baseInfo);
                    break;
                case COMMAND_HANDLER_CASE.PLAYER_MOVE:
                    PlayerMoveHandler(baseInfo);
                    break;
                case COMMAND_HANDLER_CASE.PORTAL_RESET_POSITION:
                    PortalResetPositionHandler(baseInfo);
                    break;
                case COMMAND_HANDLER_CASE.PORTAL_TRIGGER:
                    PortalTriggerHandler(baseInfo);
                    break;
                case COMMAND_HANDLER_CASE.NULL:
                    break;
            }
        }

        private void PortalTriggerHandler(BaseInfo _info)
        {
            // Subject Instance: Portal
            Portal portal = _info.GetInstance<Portal>();
            InteractInfo info = _info as InteractInfo;

            var creationTick = info.after;
            var targetPortal = portal.theOtherPortal;
            var targetPoint = targetPortal.position;

            Debug.Log(string.Format(
                "Handler: Create new Timeline at tick {0} (by {1})",
                currentTick,
                portal.name
                ));

            Record(_info);

            if (canCreateTimeline)
            {

                // TODO: This is totally bullshit here but we will fix here
                Player player = timelineManager.instancesList[0] as Player;
                Observable.Timer(TimeSpan.FromSeconds(player.baseMoveTime/2))
                    .Subscribe(_ =>
                    {
                        timelineManager.CreateNewActivePlayer(targetPoint);
                        timelineManager.CreateTimeline(creationTick); // TEMP
                        canCreateTimeline = false;
                    }
                    ).AddTo(this);         
            }
           


        }

        private void PortalResetPositionHandler(BaseInfo _info)
        {
            // Subject Instance: Portal
            Portal portal = _info.GetInstance<Portal>();
            MoveInfo info = _info as MoveInfo;
            Debug.Log(string.Format(
                "Handler: Tele Portal From {0} to {1}",
                    info.start.ToString(),
                    info.end.ToString()
                    ));

            Record(_info);
        }

        private void PlayerMoveHandler(BaseInfo _info)
        {
            // Subject Instance: Player
            Player player = _info.GetInstance<Player>();
            MoveInfo info = _info as MoveInfo;

            Debug.Log(string.Format(
                "Handler: Record Move Info: Start {0} -> End {1}",
                    info.start.ToString(),
                    info.end.ToString()
                    ));

            // Handler: Goto Next Tick after saving
            Record(_info);
            
        }

        private void ItemTriggerHandler(BaseInfo _info)
        {
            // Subject Instance: Item
            Item item = _info.GetInstance<Item>();
            InteractInfo info = _info as InteractInfo;

            // Much to be Implemented Here
            Record(_info);
        }


        #endregion


        #region PLAY


        

        public void PlayTest()
        {
            Play();
        }


        /// <summary>
        /// Play the whole tickInfodic with desinated order
        /// </summary>
        /// <param name="isReverse"></param>
        public void Play(bool isReverse = false)
        {
            StartCoroutine(Play(1, tickInfoDic.Count, 0.3f, isReverse));
        }


        public IEnumerator Play(int startTick, int endTick, float intervalTime, bool isReverse = false)
        {

            isPlaying = true;
            // Reverse
            if (isReverse)
            {
                for (var i = endTick; i >= startTick; i--)
                {
                    Play(i, isReverse); // isReverse == true
                    yield return new WaitForSeconds(intervalTime);
                }

                isPlaying = false;

            }

            // Not Reverse
            else
            {

                for (var i = startTick; i <= endTick; i++)
                {
                    Play(i, isReverse); // isReverse == false
                    yield return new WaitForSeconds(intervalTime);
                }

                isPlaying = false;
            }
        }

        public void Play(int tick, bool isReverse = false)
        {
            if (tickInfoDic.ContainsKey(tick))
            {
                Play(tickInfoDic[tick], isReverse);
            } else
            {
                Debug.LogWarning(string.Format(
               "Can not find tick {0} Info in current timeline",
               tick));
                return;
            }
        }
        

        /// <summary>
        /// Play the tick content with positive or reverse order
        /// </summary>
        /// <param name="_tickInfo"></param>
        /// <param name="isReverse"></param>
        public void Play(TickInfo _tickInfo, bool isReverse = false)
        {
            // Play All the Content at the same time (Physics Time)


            foreach(var baseInfo in _tickInfo.tickInfoList)
            {
                Play(baseInfo, isReverse);
            }



        }

        public void Play(BaseInfo baseInfo, bool isReverse = false)
        {
            switch (BaseInfo2Case(baseInfo))
            {
                case COMMAND_HANDLER_CASE.ITEM_TRIGGER:
                    PortalTriggerPlayer(baseInfo, isReverse);
                    break;
                case COMMAND_HANDLER_CASE.PLAYER_MOVE:
                    PlayerMovePlayer(baseInfo, isReverse);
                    break;
                case COMMAND_HANDLER_CASE.PORTAL_RESET_POSITION:
                    PortalResetPositionPlayer(baseInfo, isReverse);
                    break;
                case COMMAND_HANDLER_CASE.PORTAL_TRIGGER:
                    PortalTriggerPlayer(baseInfo, isReverse);
                    break;
            }

            Debug.Log(baseInfo.ToString());
        }


        private void PortalTriggerPlayer(BaseInfo _info, bool isReverse = false)
        {
            // Subject Instance: Portal
            Portal portal = _info.GetInstance<Portal>();
            InteractInfo info = _info as InteractInfo;


        }

        private void PortalResetPositionPlayer(BaseInfo _info, bool isReverse = false)
        {
            // Subject Instance: Portal
            Portal portal = _info.GetInstance<Portal>();
            MoveInfo info = _info as MoveInfo;
        }

        private void PlayerMovePlayer(BaseInfo _info, bool isReverse = false)
        {
            // Subject Instance: Player
            Player player = _info.GetInstance<Player>();
            MoveInfo info = _info as MoveInfo;

            player.timelineState = Instance.TimelineState.READ;

            Point start, end;
            if (!isReverse)
            {
                start = info.start;
                end = info.end;
            } else
            {
                start = info.end;
                end = info.start;
            }

            // Core
            player.position = start;
            player.target = end;
            player.Move(end);

            player.timelineState = Instance.TimelineState.WRITE;



        }

        private void ItemTriggerPlayer(BaseInfo _info, bool isReverse = false)
        {
            // Subject Instance: Item
            Item item = _info.GetInstance<Item>();
            InteractInfo info = _info as InteractInfo;
        }

        #endregion


        #region TOOLS


        /// <summary>
        /// Transfer baseInfo to event handler case
        /// </summary>
        /// <param name="baseInfo"></param>
        /// <returns></returns>
        private COMMAND_HANDLER_CASE BaseInfo2Case(BaseInfo baseInfo)
        {
            var instance = baseInfo.instance;

            var case_PlayerMove =
                    (baseInfo is MoveInfo) &&
                    (instance is Player);
            var case_PortalResetPosition =
                    (baseInfo is MoveInfo) &&
                    (instance is Portal);
            var case_ItemTrigger =
                    (baseInfo is InteractInfo) &&
                    (instance is Item) &&
                    !(instance is Portal);
            var case_PortalTrigger =
                    (baseInfo is InteractInfo) &&
                    (instance is Portal);

            if (case_PlayerMove)
            {
                return COMMAND_HANDLER_CASE.PLAYER_MOVE;
            } 

            else if (case_PortalResetPosition)
            {
                return COMMAND_HANDLER_CASE.PORTAL_RESET_POSITION;
            }

            else if(case_ItemTrigger)
            {
                return COMMAND_HANDLER_CASE.ITEM_TRIGGER;
            }

            else if (case_PortalTrigger)
            {
                return COMMAND_HANDLER_CASE.PORTAL_TRIGGER;
            } else
            {
                Debug.LogWarning("No match cases for this baseInfo");
                return COMMAND_HANDLER_CASE.NULL;
            }
        }


        #endregion


    }





    #region INFO TYPE


    [System.Serializable]
    public class TickInfoDic : SerializableDictionaryBase<int, TickInfo> {}



    /// <summary>
    /// A single TickInfo (List) that store multiple BaseInfo to remember all motions in 1 tick
    /// </summary>
    [System.Serializable]
    public class TickInfo
    {
        public List<BaseInfo> tickInfoList;

        public TickInfo()
        {
            tickInfoList = new List<BaseInfo>();
            
        }

        public BaseInfo this[int index]
        {
            get
            {
                return tickInfoList[index];
            }

            set
            {
                tickInfoList[index] = value;
            }
        }

        
        public void Add(BaseInfo info)
        {
            tickInfoList.Add(info);
        }

        public void Remove(BaseInfo info)
        {
            tickInfoList.Remove(info);
        }

       

    }

    /// <summary>
    /// Base of Infotype to be stored in tickInfoList
    /// </summary>
    [System.Serializable]
    public class BaseInfo
    {
        public Instance instance;
        public bool isTickEnd = true; // Default to be true


        public BaseInfo()
        {
            instance = null;
            isTickEnd = true;

        }

        public BaseInfo(Instance instance)
        {
            this.instance = instance;
            this.isTickEnd = true;
        }

        public BaseInfo(Instance instance, bool isTickEnd)
        {
            this.instance = instance;
            this.isTickEnd = isTickEnd;
        }

        /// <summary>
        /// Return the instance with T type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>() where T : Instance
        {
            var _instance = this.instance;
            var result = _instance is T;
            if (result)
            {
                T instance = _instance as T;
                return instance;
            } else
            {
                Debug.LogWarning(string.Format(
                    "Can not Get Instance of Type {0}", 
                    typeof(T).ToString()
                    ));
                return null;
            }
           
        }
    }



    /// <summary>
    /// Info: {instance} move from {start} to {end}. For Player
    /// </summary>
    [System.Serializable]
    public class MoveInfo : BaseInfo
    {
        public Point start;
        public Point end;

        public MoveInfo()
        {
            instance = null;
            start = new Point();
            end = new Point();
        }

        public MoveInfo(Instance instance, Point start, Point end, bool isTickEnd = true): base(instance, isTickEnd)
        {
            this.start = start;
            this.end = end;
        }
    }

    /// <summary>
    /// Info : {instance} is set to be state {result}. For Item
    /// </summary>
    [System.Serializable]
    public class InteractInfo : BaseInfo
    {
        public int before;
        public int after;

        public InteractInfo()
        {
            before = 0;
            after = 0;
        }

        public InteractInfo(Instance instance, int before, int after, bool isTickEnd = true): base(instance, isTickEnd)
        {
            this.before = before;
            this.after = after;
        }
    }

    #endregion





}
