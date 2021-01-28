using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UniRx;




namespace FourFrame.TopDown
{
    public class Portal : Item
    {


        [Header("Portal Settings")]
        public Portal theOtherPortal;
        public int creationTick;


        protected override void OnInteract(Instance _ins)
        {
            if(_ins.gameObject.tag == "Player")
            {
                Player player = _ins.gameObject.GetComponent<Player>();
                AddReactiveInstance(player, this);
            }
        }


        protected override void OnRelated(List<BaseInfo> _infoList)
        {
            InteractInfo _info = new InteractInfo(this, 0, theOtherPortal.creationTick);
            _infoList.Add(_info);

            OnCommandAll(_infoList);

        }
        
    }

    

}
