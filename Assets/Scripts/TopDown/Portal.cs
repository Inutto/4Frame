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


        private Player player;


        protected override void OnInteract(Instance _ins)
        {
            if (isActive)
            {
                if (_ins.gameObject.tag == "Player")
                {
                    player = _ins.gameObject.GetComponent<Player>();
                    AddReactiveInstance(player, this);
                }

                isActive = false;

                Observable.Timer(TimeSpan.FromSeconds(1f))
                    .Where(_ => isActive == false)
                    .Subscribe(_ => isActive = true)
                    .AddTo(this);
            } 

        }


        protected override void OnRelated(List<BaseInfo> _infoList)
        {
            InteractInfo _info = new InteractInfo(this, 0, theOtherPortal.creationTick);
            _infoList.Add(_info);
            RemoveReactiveInstance(player, this);
            OnCommandAll(_infoList);

            
        }
        
    }

    

}
