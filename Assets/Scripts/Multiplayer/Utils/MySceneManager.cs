using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using UnityEngine.UI;
using TMPro;


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



        #endregion

        private void Start()
        {

        }

        public void loadMenuScene()
        {
            SceneManager.LoadScene(0);
        }

        public void updateScores(int score_p1,int score_p2)
        {
            _scoreText_p1.text = score_p1.ToString();
            _scoreText_p2.text = score_p2.ToString();
        }

        public void updateClockCountDown(float countDown)
        {
            _ClockText.text = countDown.ToString();
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



    }
}

