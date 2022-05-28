using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace networking_2
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {
        public static NetworkPlayer Local { get; set; }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Debug.Log("You have spawn");
            }
            else Debug.Log("Other player has loged");
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
            {
                Runner.Despawn(Object);
            }
        }

    }

}
