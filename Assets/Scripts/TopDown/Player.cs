using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace FourFrame.TopDown
{

    public class Player : Instance
    {

        [Space(2)]
        [Header("Player Attributes")]
        public GameObject player;
        public string state; // used by FSM
        public Point target;
        public bool hasKey;



        #region DEBUG


        public void MoveUp()
        {
            Move(new Point(position.x, position.y + 1));
        }

        public void MoveDown()
        {
            Move(new Point(position.x, position.y - 1));
        }

        public void MoveLeft()
        {
            Move(new Point(position.x - 1, position.y));
        }

        public void MoveRight()
        {
            Move(new Point(position.x + 1, position.y));
        }

        public void TeleUp()
        {
            Teleport(new Point(position.x, position.y + 3));
        }


        #endregion

    }

}


