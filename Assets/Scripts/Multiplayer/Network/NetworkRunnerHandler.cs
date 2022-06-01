using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;
using System;
using local;

namespace Multiplayer
{
    public class NetworkRunnerHandler : MonoBehaviour
    {
        public NetworkRunner networkRunnerPrefab;
        public RoomManager roomManager;

        NetworkRunner networkRunner;

        // Start is called before the first frame update
        void Start()
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";
        }

        public void Init()
        {
            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex + 1, null);

            Debug.Log($"Server NetworkRunner started");
        }

        protected virtual Task InitializeNetworkRunner (NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
        {
            var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneObjectProvider>().FirstOrDefault();

            if(sceneObjectProvider == null)
            {
                // Recoje los objetos que existan en la escena y sean networked 
                sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            }

            runner.ProvideInput = true;

            return runner.StartGame(new StartGameArgs
            { 
                GameMode = gameMode,
                Address = address,
                Scene = scene,
                PlayerCount = roomManager.maxPlayers,
                SessionName = roomManager.roomName,
                Initialized = initialized,
                SceneObjectProvider = sceneObjectProvider
            });
     
        }

#if UNITY_EDITOR
        [ContextMenu("Activar multijugador")]
        public void activeMultiplayer()
        {
            this.Init();
        }

#endif


    }

}
