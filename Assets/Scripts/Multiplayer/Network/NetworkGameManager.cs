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

        [Header("Componentes HUD")]
        [SerializeField] TextMeshProUGUI _timerText;
        [SerializeField] float gameMinutes;
        [SerializeField] TextMeshProUGUI _scoreP1Text;
        [SerializeField] TextMeshProUGUI _scoreP2Text;
        [SerializeField] GameObject _countDownObj;
        [SerializeField] float _countDownTime;

        [Header("Mensajes Network")]
        [SerializeField] GameObject waitinPlayerMensaje;

        [Space(2f)]
        [Header("COMPONENTES")]
        [SerializeField] BallSpawner _ballSpawner;
        [SerializeField] RoomManager roomManager;

        private bool _isGameStarted;       


        [HideInInspector]
        public int playersCount => Runner.ActivePlayers.ToList().Count;


        // Start is called before the first frame update
        public override void Spawned()
        {
            _countDownObj.SetActive(false);
            _isGameStarted = false;
            _ballSpawner.turnOFF_M();
        }

        // Update is called once per frame
    
        public override void FixedUpdateNetwork()
        {
            //IEnumerable<PlayerRef> ActivePlayers = Runner.ActivePlayers.ToList();
            
            awaitingToPlayers();

            if (!_isGameStarted) 
                return;

            StartCoroutine(StartcountDown());
        }

        /// <summary>
        /// La partida no comienza hasta que esten todos los jugadores listos
        /// </summary>
        public void awaitingToPlayers()
        {

            if (playersCount < roomManager.maxPlayers)
            {
                waitinPlayerMensaje.SetActive(true);
                return;
            }

            if (waitinPlayerMensaje.activeInHierarchy)
                waitinPlayerMensaje.SetActive(false);

            _isGameStarted = true;
        }

        /// <summary>
        /// Inicializamos al cuenta atrás en el HUD y cuando acaba comenzamos la partida activando el Spawner de Bolas.
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartcountDown()
        {
            _countDownObj.SetActive(true);
            yield return new WaitForSeconds(_countDownTime);
            _countDownObj.SetActive(false);

            _ballSpawner.Init();
        }

    }
}

