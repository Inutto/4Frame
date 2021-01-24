using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FourFrame.TopDown{

    public class Instance : MonoBehaviour
    {


        #region SUPER PROPERTY

        /// <summary>
        /// the Point position of Instance
        /// </summary>
        [SerializeField] private Point _position;
        public Point position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                SyncPosition();
            }
        }


        /// <summary>
        /// READ to complete timelineInput and WRITE to record onto timeline
        /// </summary>
        enum TimelineState { READ, WRITE }
        TimelineState state = TimelineState.READ;



        /// <summary>
        /// Using Command Mode to Update Infor to Timeline
        /// </summary>
        public event Action OnCommand;





        #endregion


        #region SUPER METHOD

        /// <summary>
        /// Use this Method to move the Instance to adjacent Point
        /// </summary>
        public virtual void Move()
        {

        }

        /// <summary>
        /// Use this Method to move the Instance freely
        /// </summary>
        public virtual void Teleport()
        {

        }

        #endregion


        #region TOOLS

        #endregion

        #region PRIVATE IMPL

        #endregion





    }

}




