using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace Multiplayer
{
    public class PhysxBallMulty : NetworkBehaviour
    {
        [SerializeField] string scoreColliderTag = "ScoreCollider";
        bool _isActivate = false;
        bool _addForce = false;
        Vector2 _forceVector = Vector2.zero;

        // Un pequeño delay para que vuelva a detectar colisiones con el jugador
        [Networked] TickTimer delayTimer { get; set; }

        //Componentes

        Rigidbody2D _ballRb;

        private void Awake()
        {
            _ballRb = GetComponent<Rigidbody2D>();
        }
        private void OnEnable()
        {
            _isActivate = true;
            delayTimer = TickTimer.CreateFromSeconds(Runner, 0.2f);
        }

        public override void FixedUpdateNetwork()
        {
            if(!_isActivate)
            {
                Deactivate();
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
                    TurnsManager.Instance.switchTurns();
                    _isActivate = false;
                }
            }
        }
    }

}
