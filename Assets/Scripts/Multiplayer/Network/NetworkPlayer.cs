using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;


namespace Multiplayer
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {
        // Variables
        [SerializeField]Material _normalMat_p1;
        [SerializeField]Material _activeMat_p1;
        [SerializeField]Material _normalMat_p2;
        [SerializeField]Material _activeMat_p2;

        SpriteRenderer _spriteRender;


        [Networked] private NetworkBool _isPlayer1 { get; set; }

        // Network Variables
        public static NetworkPlayer LocalPlayer { get; set; }

        [Networked(OnChanged = nameof(setTurn))]
        public NetworkBool myTurn { get; set; }

        // Other componentes

        [SerializeField] Collider2D _playerCollider;

        public override void Spawned()
        {
            _spriteRender = GetComponentInChildren<SpriteRenderer>();
            //_spriteRender.material = _normalMat;

            if (Object.HasInputAuthority)
            {
                LocalPlayer = this;
                Debug.Log("Spawned Local Player");  
            }
            else
                Debug.Log("Spawned remote player");
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

                player.Behaviour._spriteRender.material = player.Behaviour._isPlayer1 ? player.Behaviour._normalMat_p1 : player.Behaviour._normalMat_p2;
            }
            else
            {
                player.Behaviour._playerCollider.gameObject.SetActive(true);
                player.Behaviour._spriteRender.material = player.Behaviour._isPlayer1 ? player.Behaviour._activeMat_p1 : player.Behaviour._activeMat_p2;
            }
        }
        
        [Rpc]
        public void RPC_setUpMaterials(NetworkBool isPlayer1)
        {
            _isPlayer1 = isPlayer1;
            myTurn = isPlayer1;
        }
    }
}

