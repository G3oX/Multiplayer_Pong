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

        //Componentes

        Rigidbody2D _ballRb;

        private void Awake()
        {
            _ballRb = GetComponent<Rigidbody2D>();
        }
        private void OnEnable()
        {
            _isActivate = true;
        }

        public override void FixedUpdateNetwork()
        {
            if(!_isActivate)
            {
                Deactivate();
            }

            if(_addForce)
            {
                _ballRb.AddForce(_forceVector, ForceMode2D.Impulse);
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
            Debug.Log("COLISION CON SCORE COLLIDER");
            if (other.gameObject.tag == scoreColliderTag)
            {
                _isActivate = false;
            }
        }
    }

}
