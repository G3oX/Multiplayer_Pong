using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Multiplayer
{
    public class BallSpawner_Old : NetworkBehaviour
    {
        [SerializeField] List<NetworkObject> _ballPrefabPoll;
        [SerializeField] NetworkObject _ballPrefab;
        [SerializeField] float timeToSpawn = 10f;
        [SerializeField] float startDelay = 3f;

        [Header("SPAWN FORCE")]
        [SerializeField] float _launchForce;
        Vector2 _forceDirection = Vector2.up;
        float _randomRange = 1f;
        Rigidbody2D _bRb;


        [Networked] TickTimer delayTimer { get; set; }
        [Networked] private bool turnON { get; set; }
        private bool firstball;


        public void Init()
        {
            Invoke("turnON_M", startDelay);
        }


        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if (!turnON) return;

            // Si no hay ninguna bola activa, lanzamos una bola.
            if (checkForActiveBall() == false)
                firstball = true;

            spawnBall();
        }

        public NetworkObject GetFreeObject()
        {
            return _ballPrefabPoll.Find(item => item.gameObject.activeInHierarchy == false);
        }

        public void turnON_M()
        {
            turnON = true;
        }

        public void turnOFF_M()
        {
            turnON = false;
        }

        public bool checkForActiveBall()
        {
            bool activeBall = false;

            for (int i = 0; i < _ballPrefabPoll.Capacity; i++)
            {
                if (_ballPrefabPoll[i].gameObject.activeInHierarchy)
                {
                    activeBall = true;
                    break;
                }
            }
            return activeBall;
        }

        private void spawnBall()
        {
            if (delayTimer.ExpiredOrNotRunning(Runner) || firstball)
            {
                // Reseteamos el timer
                delayTimer = TickTimer.CreateFromSeconds(Runner, timeToSpawn);
                firstball = false;

                NetworkObject pollObject = GetFreeObject();

                if (pollObject == null)
                {
                    Debug.Log("NULL - Creamos nueva bola");
                    pollObject = Runner.Spawn(_ballPrefab, transform.position, Quaternion.identity, Object.InputAuthority);
                    _ballPrefabPoll.Add(pollObject);
                }
                else
                {
                    pollObject.gameObject.SetActive(true);
                    pollObject.GetComponent<NetworkTransform>().TeleportToPosition(transform.position);
                }

                _bRb = pollObject.GetComponent<Rigidbody2D>();
                launchBall();

            }
        }

        public void launchBall()
        {
            float dispersionRange = Random.Range(-1 * _randomRange, _randomRange);

            _forceDirection = new Vector2(dispersionRange, _forceDirection.y);

            _bRb.AddForce(_forceDirection * _launchForce, ForceMode2D.Impulse);
        }

    }
}
