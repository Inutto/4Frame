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
        [Header("Core")]
        public int currentTick; // Start with 1 (rather than 0)

        [Header("Record List")]
        [SerializeField] private List<Instance> activeInstanceList;

        [Space(2)]
        [Header("Attributes")]
        public TickInfoDic tickInfoDic = new TickInfoDic();

        private void Start()
        {
            
        }

        #region INIT


        private void SubscribeInstances()
        {
            foreach(var instance in activeInstanceList)
            {
                instance.OnCommand += InstanceCommandHandler;
            }
        }

        private void UnSubscribeInstances()
        {
            foreach(var instance in activeInstanceList)
            {
                instance.OnCommand -= InstanceCommandHandler;
            }
        }



        #endregion


        #region EVENT


        private void InstanceCommandHandler(BaseInfo baseInfo)
        {

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
            start = new Point();
            end = new Point();
        }

        public MoveInfo(Point start, Point end)
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

        public InteractInfo(bool result)
        {
            this.result = result;
        }
    }

    #endregion





}




