using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using local;
using System.Linq;
using System;

namespace Multiplayer
{
    public class NetworkGameManager : NetworkBehaviour
    {

        #region VARIABLES

        [Header("Varialbes de elementos del HUD")]
        [Tooltip("Duración de la partida en segundos")]
        [SerializeField] float _gameSeconds;
        [SerializeField] float _startCountDownTime;
        [Networked] private float _clockTimer { get; set; }

        [Space(2f)]
        [Header("COMPONENTES")]
        [SerializeField] BallSpawner _ballSpawner;
        [SerializeField] RoomManager _roomManager;
        [SerializeField] MySceneManager _mySceneManager;
        [SerializeField] GameObject _socreBoxCollider;

        [Space(2f)]        
        [Header("DIFICULATAD CONFIG")]
        [Header("Intervalos de aumento")]

        #region DIFFICULTY SECTION

        [Range(0, 1)]
        [Tooltip("Momento de la partida en el que incrementamos la dificultad")]
        [SerializeField] float increaseDifficulty1At;      
        [Range(0, 1)]
        [Tooltip("Segundo aumento de dificultad. Asignar valor superior al 1")]
        [SerializeField] float increaseDifficulty2At;       
        [Range(0, 1)]
        [Tooltip("Tercer aumento de dificutlad. Asigar valor superior al 2")]
        [SerializeField] float increaseDifficulty3At;

        [Header("Dificultad por intervalo")]

        [Range(4, 10)]
        [Tooltip("Nuevo tiempo de delay para spawner bolas nuevas")]
        [SerializeField] float repawnDelayDifficulty1;
        [Range(4, 10)]
        [SerializeField] float repawnDelayDifficulty2;
        [Range(4, 10)]
        [SerializeField] float repawnDelayDifficulty3;

        #endregion

        // GAME STATE VARIABLES
        [Networked] private NetworkBool _isGameStarted { get; set; }
        [Networked] private NetworkBool _startCountDownFinished { get; set; }
        [Networked] private NetworkBool _isGameEnd { get; set; }

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
            _isGameEnd = false;
        }

        public override void FixedUpdateNetwork()
        {
            //IEnumerable<PlayerRef> ActivePlayers = Runner.ActivePlayers.ToList();

            if (_isGameEnd)
            {
                //Make sure if there is a live ball it does not score.
                _socreBoxCollider.SetActive(false);

                //MENU
                _mySceneManager.switch_gameOverMenu(true);
                if (!Runner.IsServer)
                    _mySceneManager.switch_disconectButton(false);

                //SHOW SCORES
                _ballSpawner.turnOFF_M();
                if (score_p1 > score_p2)
                    _mySceneManager.RPC_setWinnerName("WINNER PLAYER 1:  " + score_p1 + " - " + score_p2);
                else if(score_p1 < score_p2)
                    _mySceneManager.RPC_setWinnerName("WINNER PLAYER 2:  " + score_p2 + " - " + score_p1);
                else
                    _mySceneManager.RPC_setWinnerName("DRAW");
                
                
                return;
            }

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
                updateDifficulty();
                checkEndGame();
            }         
        }

        private void checkEndGame()
        {
            if (_clockTimer <= 0 && score_p2 != score_p1)
            {
                _isGameEnd = true;
                _mySceneManager.updateClockCountDown(0, 0);
            }
            else if(_clockTimer <= 0 && score_p2 == score_p1)
                _clockTimer += 30;
                
        }

        private void updateDifficulty()
        {
            if (_clockTimer <= _gameSeconds * (1 - increaseDifficulty3At))
            {
                _ballSpawner.RPC_changeSpawnTimeDelay(repawnDelayDifficulty3);
                Debug.Log("DIFICULTY UPDATED - New spawn delay: " + repawnDelayDifficulty3);
            }
            else if (_clockTimer <= _gameSeconds * (1 - increaseDifficulty2At))
            {
                _ballSpawner.RPC_changeSpawnTimeDelay(repawnDelayDifficulty2);
                Debug.Log("DIFICULTY UPDATED - New spawn delay: " + repawnDelayDifficulty2);
            }
            else if (_clockTimer <= _gameSeconds * (1 - increaseDifficulty1At))
            {
                _ballSpawner.RPC_changeSpawnTimeDelay(repawnDelayDifficulty1);
                Debug.Log("DIFICULTY UPDATED - New spawn delay: " + repawnDelayDifficulty1);
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

