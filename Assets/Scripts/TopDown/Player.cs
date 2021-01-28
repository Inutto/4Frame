using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FourFrame.TopDown
{

    public class Player : Instance
    {

        [Space(4)]
        [Header("Player Attributes")]
        public GameObject player;
        public string state; // used by FSM
        public Point target;

        private void Awake()
        {
            isActive = true;
        }


        protected override void Start()
        {
            // Init 
            base.Start();
            player = this.gameObject;
            target = new Point();
            state = "idle";

            // Player default to be true

            // isActive for Player: 
            //  - true -> Control and Record Brain
            //  - false -> Other Timeline and in READ mode
            
            timelineState = TimelineState.WRITE;
        }

        private void Update()
        {
            CheckState();
            switch (state)
            {
                case "idle":
                    if (timelineState == TimelineState.WRITE && isActive) CheckMovementInput();
                    break;
                case "move":
                    break;

            }
        }

        #region FUNC

        private void CheckMovementInput()
        {

            var input_h = Input.GetAxisRaw("Horizontal");
            var input_v = Input.GetAxisRaw("Vertical");

            if (input_h != 0)
            {
                Point offset = new Point((int)input_h, 0);
                target = position + offset;
                Debug.Log("Input: Player Move To Target: " + target.ToString());
                Move(target);
            }
            else if (input_v != 0)
            {
                Point offset = new Point(0, (int)input_v);
                target = position + offset;
                Debug.Log("Input: Player Move To Target: " + target.ToString());
                Move(target);
            }
        }

        private void CheckState()
        {


            switch (state)
            {
                case "move":
                    // move -> idle
                    var currentPos = gameObject.transform.position;
                    var targetPos = GridMap.Instance.Point2World(target);

                    if (Vector2.Distance(currentPos, targetPos) <= (GridMap.Instance.unit / targetMoveCheckRatio))
                    {
                        position = target; // already reached target
                        state = "idle";
                    }
                    break;

                case "idle":
                    // Nothing needed tobe done
                    break;
            }

        }

        protected override void MoveImpl(Point _pos)
        {
            // base.MoveImpl(_pos);

            var targetPos = GridMap.Instance.Point2World(_pos);
            
            state = "move";
            LeanTween.move(this.gameObject, targetPos, baseMoveTime).setEaseOutCirc();

            
           
        }


        #endregion


        #region DEBUG


       


        #endregion

    }

}


