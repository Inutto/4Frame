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

        [Header("Record List")]
        [SerializeField] public List<Instance> instancesList;

        private void Start()
        {
            UpdateInstanceList();
        }




        #region INIT

        private void UpdateInstanceList()
        {
            // TEMP
            
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

        private Portal GetPortal(string portalType)
        {
            // TODO
            return null;
        }

        #endregion


        #region METHODS

        public void CreateTimeline()
        {

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

