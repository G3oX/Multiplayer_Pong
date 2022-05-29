using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Multiplayer
{
    public class BallSpawner : NetworkBehaviour
    {
        [SerializeField] List<NetworkObject> _ballPrefabPoll;
        [SerializeField] NetworkObject _ballPrefab;
        [SerializeField] float timeToSpawn = 10f;
        [SerializeField] float startDelay = 3f;

        [Networked] TickTimer delayTimer { get; set; }
        private bool turnON = false;
        private bool firstball;


        public void Start()
        {
            Invoke("turnON_M", startDelay);
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if (!turnON) return;

            if (checkForActiveBall() == false)
                firstball = true;

            if(delayTimer.ExpiredOrNotRunning(Runner) || firstball)
            {
                // Reseteamos el timer
                delayTimer = TickTimer.CreateFromSeconds(Runner, timeToSpawn);
                firstball = false;

                NetworkObject pollObject = GetFreeObject();

                if (pollObject == null)
                {
                    Debug.Log("NULL - Creamos nueva bola");
                    _ballPrefabPoll.Add(Runner.Spawn(_ballPrefab, transform.position, Quaternion.identity, Object.InputAuthority));
                }
                else
                {
                    pollObject.gameObject.SetActive(true);
                    pollObject.GetComponent<NetworkTransform>().TeleportToPosition(transform.position);
                }

            }

            #region OBSOLETO

            //if(timer >= timeToSpawn)
            //{

            //    NetworkObject pollObject = GetFreeObject();

            //    if (pollObject == null)
            //    {
            //        Debug.Log("NULL - Creamos nueva bola");
            //        _ballPrefabPoll.Add(Runner.Spawn(_ballPrefab, transform.position, Quaternion.identity, Object.InputAuthority));
            //    }
            //    else
            //    {
            //        pollObject.gameObject.SetActive(true);
            //        pollObject.transform.position = transform.position;
            //    }

            //    timer = 0f;
            //    return;
            //}

            //timer += Runner.DeltaTime;

            #endregion
        }

        public NetworkObject GetFreeObject()
        {
            return _ballPrefabPoll.Find(item => item.gameObject.activeInHierarchy == false);
        }

        public void turnON_M()
        {
            turnON = true;
        }

        public bool checkForActiveBall()
        {
            bool activeBall = false;

            for (int i = 0; i < _ballPrefabPoll.Capacity ; i++)
            {
                if(_ballPrefabPoll[i].gameObject.activeInHierarchy)
                {
                    activeBall = true;
                    break;
                }
            }
            return activeBall;
        }
    }
}

