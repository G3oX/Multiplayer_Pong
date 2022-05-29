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
        [SerializeField] float timeToSpawn = 2f;

        [Networked] TickTimer delayTimer { get; set; }


        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {

            if(delayTimer.ExpiredOrNotRunning(Runner))
            {
                // Reseteamos el timer
                delayTimer = TickTimer.CreateFromSeconds(Runner, timeToSpawn);

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
    }
}

