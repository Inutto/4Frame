using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;




namespace FourFrame.TopDown
{
    public class Portal : Item
    {
        protected override void OnInteract(Instance _ins)
        {
            if(_ins.gameObject.tag == "Player")
            {
                // Tele Player to Another Portal after move complete

                // OnCommand For Next Tick?
            }
        }
    }

}
