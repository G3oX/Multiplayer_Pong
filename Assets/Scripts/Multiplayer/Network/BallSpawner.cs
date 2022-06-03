
#region OLD SCRIPT THAT WORKS
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Fusion;

//namespace Multiplayer
//{
//    public class BallSpawner : NetworkBehaviour
//    {
//        [SerializeField] List<NetworkObject> _ballPrefabPoll;
//        [SerializeField] NetworkObject _ballPrefab;
//        [SerializeField] float timeToSpawn = 10f;
//        [SerializeField] float startDelay = 3f;

//        [Header("SPAWN FORCE")]
//        [SerializeField] float _launchForce;
//        Vector2 _forceDirection = Vector2.up;
//        float _randomRange = 1f;
//        Rigidbody2D _bRb;


//        [Networked] TickTimer delayTimer { get; set; }
//        [Networked] private bool turnON { get; set; }
//        private bool firstball;


//        public void Init()
//        {
//            Invoke("turnON_M", startDelay);
//        }


//        // Update is called once per frame
//        public override void FixedUpdateNetwork()
//        {
//            if (!turnON) return;

//            // Si no hay ninguna bola activa, lanzamos una bola.
//            if (checkForActiveBall() == false)
//                firstball = true;

//            spawnBall();
//        }

//        public NetworkObject GetFreeObject()
//        {
//            return _ballPrefabPoll.Find(item => item.gameObject.activeInHierarchy == false);
//        }

//        public void turnON_M()
//        {
//            turnON = true;
//        }

//        public void turnOFF_M()
//        {
//            turnON = false;
//        }

//        public bool checkForActiveBall()
//        {
//            bool activeBall = false;

//            for (int i = 0; i < _ballPrefabPoll.Capacity; i++)
//            {
//                if (_ballPrefabPoll[i].gameObject.activeInHierarchy)
//                {
//                    activeBall = true;
//                    break;
//                }
//            }
//            return activeBall;
//        }

//        private void spawnBall()
//        {
//            if (delayTimer.ExpiredOrNotRunning(Runner) || firstball)
//            {
//                // Reseteamos el timer
//                delayTimer = TickTimer.CreateFromSeconds(Runner, timeToSpawn);
//                firstball = false;

//                NetworkObject pollObject = GetFreeObject();

//                if (pollObject == null)
//                {
//                    Debug.Log("NULL - Creamos nueva bola");
//                    pollObject = Runner.Spawn(_ballPrefab, transform.position, Quaternion.identity, Object.InputAuthority);
//                    _ballPrefabPoll.Add(pollObject);
//                }
//                else
//                {
//                    pollObject.gameObject.SetActive(true);
//                    pollObject.GetComponent<NetworkTransform>().TeleportToPosition(transform.position);
//                }

//                _bRb = pollObject.GetComponent<Rigidbody2D>();
//                launchBall();

//            }
//        }

//        public void launchBall()
//        {
//            float dispersionRange = Random.Range(-1 * _randomRange, _randomRange);

//            _forceDirection = new Vector2(dispersionRange, _forceDirection.y);

//            _bRb.AddForce(_forceDirection * _launchForce, ForceMode2D.Impulse);
//        }

//    }
//}

#endregion

#region NEW CRIPT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Multiplayer
{
    public class BallSpawner : NetworkBehaviour
    {

        #region VARIABLES

        [SerializeField] List<NetworkObject> _ballPrefabPoll;
        [SerializeField] NetworkObject _ballPrefab;
        [Tooltip("Transform del objeto donde se crearán las bolas si hiceran falta")]
        [SerializeField] Transform ballsContainer;
        [SerializeField] float _animationDelay = 2f;
        [SerializeField] public float _startFrecuencyShooting = 10f;

        [Header("SPAWN FORCE")]
        [SerializeField] float _launchForce;
        Vector2 _forceDirection = Vector2.up;
        float _randomRange = 1f;

        // Variables Networking
        [Networked] TickTimer animDelayTimer { get; set; }
        [Networked] TickTimer shotFrequencyTimer { get; set; }
        [Networked] private NetworkBool turnON { get; set; }
        [Networked] NetworkBool isActiveBall { get; set; }
        
        //[Networked] private NetworkBool _waitingToAnim { get; set; }
        bool _waitingToAnim;
        
        [Networked] float _shootingFrequency { get; set; }
        float _dispersionRange;

        #endregion

        public void Init()
        {
            Invoke("turnON_M", 3f);
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if (!turnON)
            {
                _shootingFrequency = _startFrecuencyShooting;
                _waitingToAnim = false;
                shotFrequencyTimer = TickTimer.CreateFromSeconds(Runner, _shootingFrequency);
                return;
            }

            if(Runner.IsServer)
            {
               isActiveBall = checkForActiveBall();
            }

            if ((shotFrequencyTimer.ExpiredOrNotRunning(Runner) || !isActiveBall) && !_waitingToAnim)
            {
                // Reseteamos el timer de lanzamientos y el de la animación
                shotFrequencyTimer = TickTimer.CreateFromSeconds(Runner, _shootingFrequency);
                animDelayTimer = TickTimer.CreateFromSeconds(Runner, _animationDelay);
                _waitingToAnim = true;
            }
            if (_waitingToAnim)
            {
                SpawnBall();
            }
        }

        private void SpawnBall()
        {
            // Esperamos a que la animación se reproduzca y luego iniciamos el lanzamiento
            if (animDelayTimer.ExpiredOrNotRunning(Runner))
            {
                // La animación a concluido
                _waitingToAnim = false;

                NetworkObject pollObject = GetFreeObject();

                if (pollObject == null /*&& Object.HasStateAuthority*/)
                {
                    Debug.Log("NO BALLS - CREATE NEW BALL");
                    pollObject = Runner.Spawn(_ballPrefab, transform.position, Quaternion.identity);
                    Debug.Log("New BALL: " + pollObject.gameObject);
                    // Agrupamos las nuevas bolas en el spawner
                    pollObject.transform.SetParent(ballsContainer);
                    _ballPrefabPoll.Add(pollObject);
                }
                else
                {
                    Debug.Log("GOT BALL FROM POOL");
                    pollObject.gameObject.SetActive(true);
                    pollObject.transform.localPosition = Vector3.zero;
                    pollObject.GetComponent<NetworkTransform>().TeleportToPosition(transform.position);
                }
                launchBall(pollObject);
            }
        }

        public void launchBall(NetworkObject pollObject)
        {
            _dispersionRange = Random.Range(-1 * _randomRange, _randomRange);

            _forceDirection = new Vector2(_dispersionRange, _forceDirection.y);

            pollObject.GetComponent<PhysxBallMulty>().addForceM(_forceDirection * _launchForce);
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

        /// <summary>
        /// Comprueba si hay alguna bola acitva en la escena
        /// </summary>
        /// <returns></returns>
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

        [Rpc]
        public void RPC_changeSpawnTimeDelay(float value)
        {
            Debug.Log("CHANGE DIFICULTY METHOD ->" + value);
            _shootingFrequency = value;
        }

    }
}

#endregion


