using UnityEngine;
using Fusion;
using UnityEngine.UI;

namespace networking
{
    public class MainMenu : MonoBehaviour
    {
        #region Inspector Variables

        [Header("MENU OBJECTS")]
        [SerializeField] Button _HostButton;
        [SerializeField] Button _JoinButton;
        [SerializeField] Button _ExitGameButton;
        [SerializeField] Button _TutorialButton;

        [Space(3f)]
        [Header("NETWORK OBJECTS")]
        [SerializeField] BasicSpawner _basicSpawner;

        #endregion

        public void onHostButton()
        {
            if(_basicSpawner.getRunner() == null)
            {
                _basicSpawner.StartGame(GameMode.Host);
            }
        }

        public void onJoinButton()
        {
            if (_basicSpawner.getRunner() == null)
            {
                _basicSpawner.StartGame(GameMode.Client);
            }
        }
    }
}

