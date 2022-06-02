using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace Multiplayer
{
    public class PhysxBallMulty : NetworkBehaviour
    {

        #region VARIALBES 

        [SerializeField] string scoreColliderTag = "ScoreCollider";
        [SerializeField] NetworkGameManager gameManager;

        // Variables privadas

        [Networked] NetworkBool _isActivate { get; set; }
        bool _addForce = false;
        Vector2 _forceVector = Vector2.zero;
        bool isCollision;
        // Un pequeño delay para que vuelva a detectar colisiones con el jugador
        [Networked] TickTimer delayTimer { get; set; }

        //Componentes

        Rigidbody2D _ballRb;

        #endregion

        private void Awake()
        {
            _ballRb = GetComponent<Rigidbody2D>();
        }
        private void OnEnable()
        {
            isCollision = false;
            _isActivate = true;
            //delayTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
        }

        public override void Spawned()
        {
            isCollision = false;
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
                _isActivate = true;
                _ballRb = GetComponent<Rigidbody2D>();
                gameManager = FindObjectOfType<NetworkGameManager>();
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (isCollision)
                _isActivate = false;

            if (!_isActivate)
            {
                Deactivate();
                return;
            }

            if (_addForce)
            {
                // Añadimos un retardo para que la bola pueda ser lanzada de nuevo ( Evitar lanzamientos con fuerza doble al colisionar con dos jugadores a la vez )
                if(delayTimer.ExpiredOrNotRunning(Runner))
                {
                    _ballRb.AddForce(_forceVector, ForceMode2D.Impulse);
                    delayTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
                }
                _addForce = false;
            }
        }
        public void addForceM(Vector2 forceVector)
        {
            _addForce = true;
            _forceVector = forceVector;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other) 
        {
            if(Runner.IsServer)
            {
                Debug.Log("COLISION CON SCORE COLLIDER");
                if (other.gameObject.tag == scoreColliderTag)
                {
                    // Modificamos puntuaicones jugadores
                    if (TurnsManager.Instance.isPlayer1Turn())
                        gameManager.RPC_updatePlayersScore(0, 1);
                    else
                        gameManager.RPC_updatePlayersScore(1, 0);
                    
                    // Cambiamos el turno del jugador
                    TurnsManager.Instance.switchTurns();

                    //Actualzimos estado de la bola
                    _isActivate = false;
                    isCollision = true;
                }
            }
        }
    }

}
