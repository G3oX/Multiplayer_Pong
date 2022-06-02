using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using local;
using System.Linq;


namespace Multiplayer
{
    public class NetworkGameManager : NetworkBehaviour
    {

        #region VARIABLES

        [Header("Varialbes de elementos del HUD")]
        [SerializeField] float _gameMinutes;
        [SerializeField] float _startCountDownTime;

        [Space(2f)]
        [Header("COMPONENTES")]
        [SerializeField] BallSpawner _ballSpawner;
        [SerializeField] RoomManager _roomManager;
        [SerializeField] MySceneManager _mySceneManager;

        // GAME STATE VARIABLES
        [Networked] private NetworkBool _isGameStarted { get; set; }
        [Networked] private NetworkBool _startCountDownFinished { get; set; }


        [HideInInspector]
        public int playersCount => Runner.ActivePlayers.ToList().Count;

        #endregion

        public override void Spawned()
        {
            _mySceneManager.switch_startCountDownObject(false);
            _isGameStarted = false;
            _startCountDownFinished = false;
            _ballSpawner.turnOFF_M();
            
        }

        public override void FixedUpdateNetwork()
        {
            //IEnumerable<PlayerRef> ActivePlayers = Runner.ActivePlayers.ToList();
            
            if (!_isGameStarted)
            {
                // Espseramos a todos los jugadores para iniciar la partida
                awaitingToPlayers();
                return;
            }

            // Inicializamos la cuenta atrás
            if (!_startCountDownFinished)
                StartCoroutine(StartcountDown());

            // Cuanta a trás finalizada


        }

        /// <summary>
        /// La partida no comienza hasta que esten todos los jugadores listos
        /// </summary>
        public void awaitingToPlayers()
        {

            if (playersCount < _roomManager.maxPlayers)
            {
                _mySceneManager.switch_waitingPlayerMensaje(true);
                return;
            }

            _mySceneManager.switch_waitingPlayerMensaje(false);

            _isGameStarted = true;
        }

        /// <summary>
        /// Inicializamos al cuenta atrás en el HUD y cuando acaba comenzamos la partida activando el Spawner de Bolas.
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartcountDown()
        {
            _mySceneManager.switch_startCountDownObject(true);
            yield return new WaitForSeconds(_startCountDownTime);

            Debug.Log("Finalizar corrutina");
            _mySceneManager.switch_startCountDownObject(false);

            _ballSpawner.Init();
            _startCountDownFinished = true;
        }

    }
}

