using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using UnityEngine.UI;
using TMPro;
using System.Linq;


namespace Multiplayer
{
    public class MySceneManager : NetworkBehaviour
    {
        #region VARIABLES
        [Header("Elementos HUD")]
        [SerializeField] TextMeshProUGUI _scoreText_p1;
        [SerializeField] TextMeshProUGUI _scoreText_p2;
        [SerializeField] TextMeshProUGUI _ClockText;
        [Space(2f)]
        [Header("Cuenta atrás")]
        [SerializeField] GameObject _countDownObj;
        [Space(2f)]
        [Header("Mensajes Network")]
        [SerializeField] GameObject _waitingPlayerMensaje;
        [Space(2f)]
        [Header("Game Over Menu")]
        [SerializeField] GameObject _gameOverMenuObj;
        [SerializeField] TextMeshProUGUI _winnerText;
        [SerializeField] Button _disconectButton;


        #endregion

        private void Start()
        {

        }

        public void loadMenuScene()
        {
            List<PlayerRef> ActivePlayers = Runner.ActivePlayers.ToList();
            
            if(Object.HasStateAuthority)
            {
                Debug.Log("SERVER - DESCONECTO JUGADOR 1 Y CIERRO");
                Runner.Disconnect(ActivePlayers[1]);
                Runner.Shutdown();
            }
            else
            {
                Debug.Log("NO SERVER - ME SALGO");
                //NetworkPlayer player = TurnsManager.Instance.getPlayersList()[1];
                //player.PlayerLeft(ActivePlayers[1]);
            }
        }

        public void updateScores(int score_p1,int score_p2)
        {
            _scoreText_p1.text = score_p1.ToString();
            _scoreText_p2.text = score_p2.ToString();
        }

        public void updateClockCountDown(float mins, float seconds)
        {
            _ClockText.text = string.Format("{0:00}:{1:00}", mins, seconds);
        }

        /// <summary>
        /// Metodo que activa/desactiva el objeto del mensaje Esperando a jugadores
        /// </summary>
        /// <param name="state">True para activar y False para desactivar</param>
        public void switch_waitingPlayerMensaje(bool state)
        {
            _waitingPlayerMensaje.gameObject.SetActive(state);
        }

        /// <summary>
        /// Metodo que activa/desactiva el objeto de la cuenta atrás
        /// </summary>
        /// <param name="state">True para activar y False para desactivar</param>
        public void switch_startCountDownObject(bool state)
        {
            _countDownObj.gameObject.SetActive(state);
        }

        public void switch_gameOverMenu(bool state)
        {
            _gameOverMenuObj.SetActive(state);
        }

        public void switch_disconectButton(bool state)
        {
            _disconectButton.gameObject.SetActive(state);
        }

        [Rpc]
        public void RPC_setWinnerName(string winerText)
        {
            _winnerText.text = winerText;
        }



    }
}

