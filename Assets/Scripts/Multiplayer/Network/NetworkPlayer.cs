using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace Multiplayer
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {
        // Variables

        // Network Variables
        public static NetworkPlayer Local { get; set; }

        [Networked(OnChanged = nameof(setTurn))]
        public NetworkBool myTurn { get; set; }

        // Other componentes

        [SerializeField] Collider2D _playerCollider;

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Debug.Log("Spawned Local Player");
            }
            else Debug.Log("Spawned remote player");
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
                Runner.Despawn(Object);
        }

        /// <summary>
        /// Activa o desactiva el collider del jugador cada vez que su estado de myTurn cambie
        /// </summary>
        public static void setTurn(Changed<NetworkPlayer> player)
        {
            if (!player.Behaviour.myTurn)
            {
                player.Behaviour._playerCollider.gameObject.SetActive(false);
            }
            else
                player.Behaviour._playerCollider.gameObject.SetActive(true);
        }
    }
}

