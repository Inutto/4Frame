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
        /// READ to complete timelineInput with *NO INPUT CONTROL*
        /// , and WRITE to record onto timeline with *INPUT CONTROl*
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


        /// <summary>
        /// Use this Method to behave when the Instance is been interacted
        /// </summary>
        public virtual void OnInteract()
        {

        }

        #endregion


        #region TOOLS

        /// <summary>
        /// Return the instance at desinated Point position and layermask
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_pos"></param>
        /// <param name="_layer"></param>
        /// <returns></returns>
        protected T GetInstanceAt<T>(Point _pos, LayerMask _layer) where T : Instance
        {
            Instance _ins = GetInstanceAt(_pos, _layer);
            if (_ins != null)
            {
                bool result = _ins is T;
                if (result)
                {
                    T ins = _ins as T;
                    return ins;
                }
                else
                {
                    Debug.Log(string.Format("No instance of required T Type at targetPos:") + _pos.ToString());
                    return null;
                }
            }

            else
            {
                return null;
            }
        }

        /// <summary>
        /// Is there any instance at desinated Point position?
        /// </summary>
        /// <typeparam name="T"></typeparam> Type of instance
        /// <param name="_pos"></param>
        /// <param name="_layer"></param> Layermask
        /// <returns></returns>
        protected bool IsInstanceAt<T>(Point _pos, LayerMask _layer) where T : Instance
        {
            T ins = GetInstanceAt<T>(_pos, _layer);
            if (ins != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Return true if _pos is adjacent Point position of this.position
        /// </summary>
        /// <param name="_pos"></param>
        /// <returns></returns>
        protected bool IsAdjacentPos(Point _pos)
        {
            var x_adj = Mathf.Abs((position.x - _pos.x)) == 1;
            var y_adj = Mathf.Abs((position.y - _pos.y)) == 1;

            var x_same = (position.x - _pos.x) == 0;
            var y_same = (position.y - _pos.y) == 0;

            return (x_adj && y_same || y_adj && x_same);
        }

        /// <summary>
        /// Return true if _ins.position is adjacent Point position of this.position
        /// </summary>
        /// <param name="_ins"></param>
        /// <returns></returns>
        protected bool IsAdjacentPos(Instance _ins)
        {
            return (IsAdjacentPos(_ins.position));
        }

        /// <summary>
        /// Sync Point position to World position
        /// </summary>
        protected void SyncPosition()
        {
            SyncPosition(position);
        }

        /// <summary>
        /// Sync _pos to World position
        /// </summary>
        /// <param name="_pos"></param>
        protected void SyncPosition(Point _pos)
        {
            var syncPosition = GridMap.Instance.Point2World(_pos);
            gameObject.transform.position = syncPosition;
        }






        #endregion


        #region PRIVATE IMPL


        private bool isInstanceAt(Point _pos, LayerMask _layer)
        {
            Collider2D hitCollider = GetColliderAt(_pos, _layer);
            if (hitCollider != null)
            {
                return true;
            }
            else
            {
                Debug.Log("---> No Collider at targetPos");
                return false;
            }
        }

        private Instance GetInstanceAt(Point _pos, LayerMask _layer)
        {
            Collider2D hitCollider = GetColliderAt(_pos, _layer);
            if (hitCollider != null)
            {
                Instance ins = hitCollider.gameObject.GetComponent<Instance>();
                return ins;
            }
            else
            {
                //Debug.Log("---> No Collider at targetPos");
                return null;
            }
        }

        private Collider2D GetColliderAt(Point _pos, LayerMask _layer)
        {
            var targetPos = GridMap.Instance.Point2World(_pos);
            var checkRadius = GridMap.Instance.unit / 2;

            //Debug.Log("Check Instance At target Position: " + StringPos(_pos));
            Collider2D hitCollider = Physics2D.OverlapCircle(targetPos, checkRadius, _layer);
            return hitCollider;
        }


        #endregion


    }

}