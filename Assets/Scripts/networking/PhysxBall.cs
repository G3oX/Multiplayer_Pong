using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace networking
{
    public class PhysxBall : NetworkBehaviour
    {
        [Networked] private TickTimer life { get; set; }

        public void Init()
        {
            life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        }

        public override void FixedUpdateNetwork()
        {
            if (life.Expired(Runner))
                Runner.Despawn(Object);
        }
    }

}
