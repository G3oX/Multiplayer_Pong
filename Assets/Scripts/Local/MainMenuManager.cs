using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Multiplayer;

namespace local
{
    public class MainMenuManager : MonoBehaviour
    {
        #region Componentes
        //Input Field
        [Header("SERVER NAME")]
        [Space(1f)]
        [SerializeField] TMP_InputField inputField;

        // Botones
        [Header("BUTTONS")]
        [Space(1f)]
        [SerializeField] Button hostB;
        [SerializeField] Button joinB;
        [SerializeField] Button tutorialB;
        [SerializeField] Button exitGameB;
        [SerializeField] Button goBackB;

        // CanvasGroups
        [Header("GROUPS")]
        [Space(1f)]
        [SerializeField] CanvasGroup buttonsGroup;
        [SerializeField] CanvasGroup mainMenuGroup;
        [SerializeField] CanvasGroup tutorialGroup;

        // Animacion
        [Header("ANIMATION")]
        [Space(1f)]
        [SerializeField] Animator mainMenuAnim;
        [SerializeField] Animator tutorialMenuAnim;
        [SerializeField] float delayForDeactivate;

        // Componentes
        [Header("COMPONENTES")]
        [Space(1f)]
        [SerializeField] RoomManager roomManager;
        [SerializeField] NetworkRunnerHandler networkRunnerHandler; 

        #endregion

        string serverName;


        // Start is called before the first frame update
        void Start()
        {
            Invoke("activeMainMenu", 1f);
        }

        // Update is called once per frame
        void Update()
        {
            serverName = inputField.text;
            //Debug.Log(serverName);
        }

        #region BUTTONS ACTIONS

        public void loadScene(int sceneIndex)
        {
            roomManager.roomName = serverName;
            networkRunnerHandler.Init();
            //SceneManager.LoadScene(sceneIndex);
        }

        public void exitGame()
        {
            Application.Quit();
        }

        public void activeMenu(GameObject Menu)
        {
            Menu.SetActive(true);
        }

        public void deactiveMenu(Animator anim)
        {
            anim.SetTrigger("a_Close");
        }

        #endregion

        private void activeMainMenu()
        {
            mainMenuGroup.gameObject.SetActive(true);
        }
    }
}

