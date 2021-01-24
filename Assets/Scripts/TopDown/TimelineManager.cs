using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FourFrame.TopDown
{
    public class TimelineManager : MonoBehaviour
    {
        [Header("Core")]
        public int globalTick;

        [Header("Record List")]
        [SerializeField] public List<Instance> instancesList;

        private void Start()
        {
            UpdateInstanceList();
        }




        #region INIT

        private void UpdateInstanceList()
        {

        }


        private Player GetActivePlayer()
        {
            var players = Instance.GetInstances<Player>("Player");
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



     

    }





}

