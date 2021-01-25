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

        [Header("Core")]
        public int currentTick; // Start with 1 (rather than 0) 

        [Space(2)]
        [Header("Attributes")]
        public TickInfoDic tickInfoDic = new TickInfoDic();

        private void Start()
        {
            // TODO: Replace with normal get method
            timelineManager = GameObject.Find("TimelineManager").GetComponent<TimelineManager>();
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
           

          
        }

        // TODO : Create Handlers for each case above



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

        #endregion


        #region IMPL


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
    public class TickInfoDic : SerializableDictionaryBase<int, TickInfo> { }



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




