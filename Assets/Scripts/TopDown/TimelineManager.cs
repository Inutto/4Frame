using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FourFrame.TopDown
{
    public class TimelineManager : MonoBehaviour
    {
        /// <summary>
        /// Unified and Only CURRENT tick for all timeline
        /// </summary>
        [Header("Core")]
        public int currentTick = 1;
        public Timeline main;

        [Header("Record List")]
        [SerializeField] public List<Instance> instancesList;
        [SerializeField] public List<Timeline> timelineList;

        private void Start()
        {
            main = GetComponentInChildren<Timeline>();
            timelineList.Add(main);
            UpdateInstanceList();
            main.SubscribeInstances();
        }




        #region INIT

        private void UpdateInstanceList()
        {
            instancesList.Clear();

            var player = GetActivePlayer();
            var portals = Instance.GetInstancesWithTag<Portal>("Portal");

            instancesList.Add(player);
            foreach(var portal in portals)
            {
                instancesList.Add(portal);
            }
            
        }



        private Player GetActivePlayer()
        {
            var players = Instance.GetInstancesWithTag<Player>("Player");
            foreach (var player in players)
            {
                if (player.isActive == true)
                {
                    return player;
                }
            }
            Debug.LogWarning("Timeline: Can not find active Player: No Active one");
            return null;
        }

        private Portal GetPortal()
        {
            var portals = Instance.GetInstancesWithTag<Portal>("Portal");
           
            Debug.LogWarning("Timeline: Can not find active Player: No Active one");
            return null;
        }

        #endregion


        #region METHODS

        public void CreateTimeline(int _tick)
        {

            var oldTimeline = main;
            currentTick = _tick;

            // Creation
            var newTimeline = Instantiate(oldTimeline, this.transform);
            newTimeline.tickInfoDic.Clear();
            newTimeline.InitEmptyTickInfoDic(currentTick);

           
            timelineList.Add(newTimeline);

            // Subscription
            newTimeline.SubscribeInstances();
            oldTimeline.UnSubscribeInstances();

            // Resign Main
            this.main = newTimeline;



        }

        public void CreateNewActivePlayer(Point newPosition)
        {
            var oldPlayer = GetActivePlayer();
            var newPlayer = Instantiate(oldPlayer, oldPlayer.transform.parent);

            // Old
            oldPlayer.isActive = false;
            main.UnSubscribeInstance(oldPlayer);

            // New
            newPlayer.position = newPosition;
            newPlayer.isActive = true;
            UpdateInstanceList();

        }

        public void Play()
        {
            
        }

        public void GotoTick(int tick)
        {

        }

        public void GotoNextTick()
        {
            // Play All
            foreach(var timeline in timelineList)
            {
                if (!(timeline == main))
                {
                    StartCoroutine(timeline.Play(currentTick, currentTick, 0.3f));
                }
            }

            // Next Tick
            currentTick++;
            foreach(var timeline in timelineList)
            {
                var hasRecord = timeline.tickInfoDic.ContainsKey(currentTick);

                if (hasRecord)
                {
                    var tickInfoListIsEmpty = timeline.tickInfoDic[currentTick].tickInfoList.Count == 0;

                    if (tickInfoListIsEmpty)
                    {
                        timeline.InitEmptyTickInfoDic(currentTick);
                    }
                } else
                {
                    timeline.InitEmptyTickInfoDic(currentTick);
                }
                
            }
        }

        public void GotoPreviousTick()
        {
            // Play All
            foreach (var timeline in timelineList)
            {
                StartCoroutine(timeline.Play(currentTick-1, currentTick-1, 0.3f, true));
            }

            // Next Tick
            currentTick--;
            foreach (var timeline in timelineList)
            {
                var hasRecord = timeline.tickInfoDic.ContainsKey(currentTick);

                if (hasRecord)
                {
                    var tickInfoListIsEmpty = timeline.tickInfoDic[currentTick].tickInfoList.Count == 0;

                    if (tickInfoListIsEmpty)
                    {
                        timeline.InitEmptyTickInfoDic(currentTick);
                    }
                }
                else
                {
                    timeline.InitEmptyTickInfoDic(currentTick);
                }

            }

           
        }

        #endregion


    }





}

