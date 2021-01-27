using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using BasicTools.ButtonInspector;

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
        /// isActive link with OnActive() and OnInactive()
        /// </summary>
        [SerializeField] private bool _isActive;
        public bool isActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                if(_isActive != value && value == true)
                {
                    OnActivate();
                    _isActive = true;
                } 

                else if(_isActive != value && value == false)
                {
                    OnDeactivate();
                    _isActive = false;
                }

                else
                {
                    // Do nothing
                }
            }
        }
        


        /// <summary>
        /// READ to complete timelineInput with *NO INPUT CONTROL*
        /// , and WRITE to record onto timeline with *INPUT CONTROl*
        /// </summary>
        public enum TimelineState { READ, WRITE }
        public TimelineState timelineState = TimelineState.READ;


        /// <summary>
        /// Using Command Mode to Update Infor to Timeline
        /// </summary>
        public event Action<BaseInfo> OnCommand;



        #endregion


        #region SUPER METHOD


        [Header("Movement Settings")]
        public LayerMask layerMask;
        [SerializeField] protected float baseMoveTime = 0.5f;
        [SerializeField] protected float targetMoveCheckRatio = 1000f;

        [Space(2)]
        [Header("Gizmos Settings")]
        public Vector3 drawPosition;
        public float drawRadius;

        protected virtual void Start()
        {
            AutoPosition();

            // Debug
            drawRadius = GridMap.Instance.unit / 4;
        }


       


        /// <summary>
        /// Use this Method to move the Instance to adjacent Point
        /// </summary>
        public virtual void Move(Point _pos)
        {

            switch (timelineState)
            {
                case TimelineState.READ: // Follow the play of timeline
                    DirectMove();
                    break;
                case TimelineState.WRITE: // Record info to timeline
                    MovementCheck();
                    break;
            }


            void DirectMove()
            {
                // Movement
                UpdateGismosInfo(_pos);
                MoveImpl(_pos);
                return;
            }

            void Nofity()
            {
                // Notify Timeline
                MoveInfo moveInfo = new MoveInfo(this, position, _pos);

                if (reactiveInstance != null)
                {
                    // Transfer Command To reactive instance
                    reactiveInstance.OnRelated(new MoveInfo[] { moveInfo });
                }
                else
                {
                    // Directly Notify Timeline
                    OnCommand(moveInfo);
                }

            }

            // Core
            void MovementCheck()
            {
                // Distant
                if (!IsAdjacentPos(_pos))
                {
                    Debug.Log("Can not move for not adjacent Position");
                    return;
                }

                // Wall
                else if (IsInstanceAt<Wall>(_pos, GridMap.Instance.collLayer))
                {
                    Debug.Log("Can not move to pos for wall");
                    return;

                }

                // Item
                else if (IsInstanceAt<Item>(_pos, GridMap.Instance.itemLayer))
                {
                    Item item = GetInstanceAt<Item>(_pos, GridMap.Instance.itemLayer);
                    item.OnInteract(this);
                    Nofity();
                    DirectMove();
                    return;
                }

                // Normal Movement
                else
                {
                    Nofity();
                    DirectMove();
                }
            }

           
        }

        
        /// <summary>
        /// Use this Method to move the Instance freely
        /// </summary>
        public virtual void Teleport(Point _pos)
        {

            switch (timelineState)
            {
                case TimelineState.READ: // Follow the play of timeline
                    DirectTeleport();
                    break;
                case TimelineState.WRITE: // Record info to timeline
                    TeleportCheck();
                    break;
            }


            void TeleportCheck()
            {
                // Wall
                if (IsInstanceAt<Wall>(_pos, GridMap.Instance.collLayer))
                {
                    Debug.Log("Can not tele to pos for wall");
                    return;

                }

                // Item
                else if (IsInstanceAt<Item>(_pos, GridMap.Instance.itemLayer))
                {
                    Item item = GetInstanceAt<Item>(_pos, GridMap.Instance.itemLayer);
                    item.OnInteract(this);
                    Nofity();
                    DirectTeleport();
                    return;
                }

                else
                {
                    Nofity();
                    DirectTeleport();
                }
            }

            void DirectTeleport()
            {
                // Teleport
                UpdateGismosInfo(_pos);
                TeleportImpl(_pos);
                return;
            }

            void Nofity()
            {
                // Notify Timeline
                MoveInfo moveInfo = new MoveInfo(this, position, _pos);

                if (reactiveInstance != null)
                {
                    // Transfer Command To reactive instance
                    reactiveInstance.OnRelated(new MoveInfo[] { moveInfo });
                }
                else
                {
                    // Directly Notify Timeline
                    OnCommand(moveInfo);
                }

            }
        }



        #endregion


        #region PROTECTED METHOD (PURE VIRTUAL AREA)

        /// <summary>
        /// The instance to notify with Command
        /// </summary>
        [Header("Relation Settings")]
        protected Instance reactiveInstance;

        /// <summary>
        /// Overrided by childed or use default
        /// </summary>
        /// <param name="_pos"></param>
        protected virtual void MoveImpl(Point _pos)
        {
            // Implemented by child, or use this Default without:
            var targetPos = GridMap.Instance.Point2World(_pos);
            LeanTween.move(this.gameObject, targetPos, baseMoveTime)
                .setEaseOutCirc();

            Observable.Timer(TimeSpan.FromSeconds(baseMoveTime))
                .Subscribe(_ => this.position = _pos)
                .AddTo(this);   
        }


        /// <summary>
        /// Overrided by childed or use default
        /// </summary>
        /// <param name="_pos"></param>
        protected virtual void TeleportImpl(Point _pos)
        {
            var targetPos = GridMap.Instance.Point2World(_pos);
            this.position = _pos;
        }


        /// <summary>
        /// Use this Method to behave when the Instance is been interacted
        /// </summary>
        protected virtual void OnInteract(Instance interactInstance)
        {
            // Implemented by child.
        }

        /// <summary>
        /// Called when isActive turns from false to true
        /// </summary>
        protected virtual void OnActivate()
        {
            // Implemented by child.
        }

        /// <summary>
        /// Called when isActive turns from true to false
        /// </summary>
        protected virtual void OnDeactivate()
        {
            // Implemented by child.
        }


        /// <summary>
        /// If this player has related with another instance, then all the 
        /// Command will be sent to this instance to deal with in this func
        /// </summary>
        protected virtual void OnRelated(BaseInfo[] _infoGroup)
        {
            // Implemented if any instance is related to current Instance: transfer message

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

        /// <summary>
        /// Auto from world position to Point position
        /// </summary>
        protected void AutoPosition()
        {
            var worldPosition = gameObject.transform.position;
            this.position = GridMap.Instance.World2Point(worldPosition);
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


        #region GLOBAL STATIC

        /// <summary>
        /// Get All Instances of type T with desinated tag
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static T[] GetInstancesWithTag<T>(string tag) where T : Instance
        {
            var _instances = GameObject.FindGameObjectsWithTag(tag);

            if (_instances.Length == 0)
            {
                Debug.LogWarning(string.Format("Timeline: Can not find active {0}: No Players", tag));
                return null;
            }
            else
            {
                T[] instances = new T[_instances.Length];
                for (int i = 0; i < _instances.Length; i++)
                {
                    T instance = _instances[i].GetComponent<T>();
                    instances[i] = instance;
                }

                return instances;

            }
        }


        #endregion


        #region DEBUG


        protected virtual void OnDrawGizmos()
        {
            // TEMP
            if (this is Player)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(drawPosition, drawRadius);
            }
                
        }

        private void UpdateGismosInfo(Point _pos)
        {
             drawPosition = GridMap.Instance.Point2World(_pos);
        }
        #endregion


       


    }

}