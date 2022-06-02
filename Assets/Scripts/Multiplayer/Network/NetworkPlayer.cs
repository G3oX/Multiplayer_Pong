using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;


namespace Multiplayer
{
    public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
    {

        #region VARIABLES
        // Variables
        [Header("MATERIALES PLAYERS")]
        [SerializeField]Material _normalMat_p1;
        [SerializeField]Material _activeMat_p1;
        [SerializeField]Material _normalMat_p2;
        [SerializeField]Material _activeMat_p2;

        // Other componentes
        [Header("COMPONENTES")]
        [SerializeField] Collider2D _playerCollider;

        // Privadas y Networking
        SpriteRenderer _spriteRender;
        [Networked] private NetworkBool _isPlayer1 { get; set; }
        public static NetworkPlayer LocalPlayer { get; set; }
        [Networked(OnChanged = nameof(setTurn))]
        public NetworkBool myTurn { get; set; }

        #endregion

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
            // Comprobamos si es mi turno
            if (!player.Behaviour.myTurn)
            {
                // Desactivamos el colider para que no pueda interactuar con las bolas
                player.Behaviour._playerCollider.gameObject.SetActive(false);
                // Asignamos el material de INACTIVO
                player.Behaviour._spriteRender.material = player.Behaviour._isPlayer1 ? player.Behaviour._normalMat_p1 : player.Behaviour._normalMat_p2;
            }
            else
            {
                // activamos el colider para que pueda interactuar con las bolas
                player.Behaviour._playerCollider.gameObject.SetActive(true);
                // Asignamos el material de ACTIVO
                player.Behaviour._spriteRender.material = player.Behaviour._isPlayer1 ? player.Behaviour._activeMat_p1 : player.Behaviour._activeMat_p2;
            }
        }
        
        [Rpc]
        public void RPC_configuraAsPlayer1(NetworkBool value)
        {
            // Consideramos este player como jugador 1
            _isPlayer1 = value;
            // EL primer jugador inicia con el turno
            myTurn = value;
        }
    }
}

