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
        [Tooltip("Duración de la partida en segundos")]
        [SerializeField] float _gameSeconds;
        [SerializeField] float _startCountDownTime;
        private float _clockTimer;

        [Space(2f)]
        [Header("COMPONENTES")]
        [SerializeField] BallSpawner _ballSpawner;
        [SerializeField] RoomManager _roomManager;
        [SerializeField] MySceneManager _mySceneManager;

        // GAME STATE VARIABLES
        [Networked] private NetworkBool _isGameStarted { get; set; }
        [Networked] private NetworkBool _startCountDownFinished { get; set; }

        [Networked(OnChanged = nameof(updateHUDscores))]
        private int score_p1 { get; set; }
        [Networked(OnChanged = nameof(updateHUDscores))]
        private int score_p2 { get; set; }

        [HideInInspector]
        public int playersCount => Runner.ActivePlayers.ToList().Count;

        #endregion

        public override void Spawned()
        {
            _mySceneManager.switch_startCountDownObject(false);
            _isGameStarted = false;
            _startCountDownFinished = false;
            _ballSpawner.turnOFF_M();
            _clockTimer = _gameSeconds;
            updateGameClock();
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
            else
            {
                // Cuanta a trás finalizada. Comienza la partida
                updateGameClock();
               
            }         
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

        public void updateGameClock()
        {
            int minutes = Mathf.FloorToInt(_clockTimer / 60f);
            int seconds = Mathf.FloorToInt(_clockTimer - minutes * 60f);

            //Debug.Log("MIN: " + (int)minutes + "SEG: " + (int)seconds);

            _mySceneManager.updateClockCountDown(minutes, seconds);
            _clockTimer -= Runner.DeltaTime;
        }

        public void resetGameClock()
        {
            _clockTimer = _gameSeconds;
        }

        /// <summary>
        /// Cada vez que una puntuación sea actualizada ejecutamos esta función que actualiza el HUD.
        /// </summary>
        /// <param name="gameManager"></param>
        public static void updateHUDscores(Changed<NetworkGameManager> gameManager)
        {
            gameManager.Behaviour._mySceneManager.updateScores(gameManager.Behaviour.score_p1, gameManager.Behaviour.score_p2);
        }

        /// <summary>
        /// Metodo que actualiza las propiedades network de las puntuaciones y las envia por RPC para segurar que se reciben. 
        /// </summary>
        /// <param name="score1"> Puntuación añadida para el jugador 1</param>
        /// <param name="score2"> Puntuación añadida para el jugador 2</param>
        [Rpc]
        public void RPC_updatePlayersScore(int score1, int score2)
        {
            score_p1 += score1;
            score_p2 += score2;
        }

    }
}

