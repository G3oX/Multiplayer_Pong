
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
        [Tooltip("Transform del objeto donde se crear�n las bolas si hiceran falta")]
        [SerializeField] Transform ballsContainer;
        [SerializeField] float _initializationDelay = 1f;
        [SerializeField] float _animationDelay = 2f;
        [SerializeField] public float _startFrecuencyShooting = 10f;

        [Header("SPAWN FORCE")]
        [SerializeField] float _launchForce;
        Vector2 _forceDirection = Vector2.up;
        float _randomRange = 1f;
        Animator _shotAnimator;


        // Variables Networking
        [Networked] TickTimer animDelayTimer { get; set; }
        [Networked] TickTimer shotFrequencyTimer { get; set; }
        [Networked] private NetworkBool turnON { get; set; }
        [Networked] NetworkBool isActiveBall { get; set; }       
        [Networked] private NetworkBool _waitingToAnim { get; set; }
        
        [Networked] float _shootingFrequency { get; set; }
        float _dispersionRange;

        #endregion
        public void Awake()
        {
            _shotAnimator = GetComponentInChildren<Animator>();
        }

        public void Init()
        {
            Invoke("turnON_M", _initializationDelay);
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            if (!turnON)
            {
                RPC_activeAndDeactiveShotAnim(false);
                _shootingFrequency = _startFrecuencyShooting;
                _waitingToAnim = false;
                shotFrequencyTimer = TickTimer.CreateFromSeconds(Runner, _shootingFrequency);
                
                return;
            }

            if(Runner.IsServer)
            {
                // Comprobamos si hay alugna bola en juego.
                isActiveBall = checkForActiveBall();
            }

            if ((shotFrequencyTimer.ExpiredOrNotRunning(Runner) || !isActiveBall) && !_waitingToAnim)
            {
                // Reseteamos el timer de lanzamientos y el de la animaci�n
                shotFrequencyTimer = TickTimer.CreateFromSeconds(Runner, _shootingFrequency);
                animDelayTimer = TickTimer.CreateFromSeconds(Runner, _animationDelay);
                _waitingToAnim = true;
                // Activamos la animaci�n
                if (Runner.IsServer)
                    RPC_activeAndDeactiveShotAnim(true);
            }
            if (_waitingToAnim)// Cuando la animaci�n haya terminado lanzamos la bola
            {
                SpawnBall();
            }
        }

        private void SpawnBall()
        {
            // Esperamos a que la animaci�n se reproduzca y luego iniciamos el lanzamiento
            if (animDelayTimer.ExpiredOrNotRunning(Runner) && Object.HasStateAuthority)
            {
                RPC_setWaitingToAnim(false);
                NetworkObject pollObject = GetFreeObject();

                if (pollObject == null /*&& Object.HasStateAuthority*/)
                {
                    RPC_CreateNewBall(pollObject);
                }
                else
                {
                    RPC_ActiveNewBall(pollObject);
                }

                launchBall(pollObject);                
                RPC_activeAndDeactiveShotAnim(false);
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

        [Rpc]
        public void RPC_activeAndDeactiveShotAnim(NetworkBool value)
        {
            _shotAnimator.SetBool("a_Shot", value);
        }

        [Rpc]
        public void RPC_isActiveBal(NetworkBool value)
        {
            isActiveBall = value;
        }

        [Rpc]
        public void RPC_ActiveNewBall(NetworkObject pollObject)
        {
            Debug.Log("GOT BALL FROM POOL");
            pollObject.gameObject.SetActive(true);
            pollObject.transform.localPosition = Vector3.zero;
            pollObject.GetComponentInChildren<TrailRenderer>().Clear();
            pollObject.GetComponent<NetworkTransform>().TeleportToPosition(transform.position);
        }

        [Rpc]
        public void RPC_CreateNewBall(NetworkObject pollObject)
        {
            Debug.Log("NO BALLS - CREATE NEW BALL");
            pollObject = Runner.Spawn(_ballPrefab, transform.position, Quaternion.identity);
            // Agrupamos las nuevas bolas en el spawner
            pollObject.transform.SetParent(ballsContainer);
            _ballPrefabPoll.Add(pollObject);
        }

        [Rpc]
        public void RPC_setWaitingToAnim(NetworkBool value)
        {
            _waitingToAnim = value;
        }

    }
}

#endregion


