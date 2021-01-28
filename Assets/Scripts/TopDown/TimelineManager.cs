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
        public int currentTick;
        public Timeline main;

        [Header("Record List")]
        [SerializeField] public List<Instance> instancesList;

        private void Start()
        {
            main = GetComponentInChildren<Timeline>();
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

            // Creation
            var newTimeline = Instantiate(oldTimeline, this.transform);
            newTimeline.tickInfoDic.Clear();
            currentTick = _tick;

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

        #endregion


    }





}

