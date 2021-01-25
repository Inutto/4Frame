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

        private TimelineManager timelineManager;
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

            SubscribeInstances();
        }

        #region INIT


        private void SubscribeInstances()
        {
            foreach(var instance in timelineManager.instancesList)
            {
                instance.OnCommand += InstanceCommandHandler;
            }
        }

        private void UnSubscribeInstances()
        {
            foreach(var instance in timelineManager.instancesList)
            {
                instance.OnCommand -= InstanceCommandHandler;
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

            // Handler: Create new timeline and restore everything to that tick

            
            
        }

        private void PortalResetPositionHandler(BaseInfo _info)
        {
            // Subject Instance: Portal
            Portal portal = _info.GetInstance<Portal>();
            MoveInfo info = _info as MoveInfo;

            // Handler: 
        }

        private void PlayerMoveHandler(BaseInfo _info)
        {
            // Subject Instance: Player
            Player player = _info.GetInstance<Player>();
            MoveInfo info = _info as MoveInfo;
        }

        private void ItemTriggerHandler(BaseInfo _info)
        {
            // Subject Instance: Item
            Item item = _info.GetInstance<Item>();
            InteractInfo info = _info as InteractInfo;
        }


        #endregion


        #region PLAY

        /// <summary>
        /// Play the tick content with positive or reverse order
        /// </summary>
        /// <param name="_tickInfo"></param>
        /// <param name="isReverse"></param>
        public void Play(TickInfo _tickInfo, bool isReverse = false)
        {

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
        private List<BaseInfo> tickInfoList;

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


        public BaseInfo()
        {
            instance = null;
        }

        public BaseInfo(Instance instance)
        {
            this.instance = instance;
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

        public MoveInfo(Instance instance, Point start, Point end): base(instance)
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
        public bool result;

        public InteractInfo()
        {
            result = false;
        }

        public InteractInfo(Instance instance, bool result): base(instance)
        {
            this.result = result;
        }
    }

    #endregion





}




