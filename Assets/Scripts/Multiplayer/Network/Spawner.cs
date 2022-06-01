using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;


namespace Multiplayer
{
    public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
    {

        #region VARIABLES

        public NetworkPlayer playerPrefab;
        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        [Header("Spawn Point")]
        [SerializeField] Vector2[] spawnPosition = new Vector2[1];
        private int spawnPositionIndex;

        [Space(2f)]
        [Header("Players Materials")]
        [SerializeField] Color p1_normalMatcolor;
        [SerializeField] Color p2_normalMatcolor;


        [Tooltip("Radio del GIZMO que representa el punto de reaparición")]
        [SerializeField] float radius;

        // privatdas
       
        
        //Otros Componentes
        CharacterInputHandler characterInputHandler;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            spawnPositionIndex = 0;
        }

        void Awake()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region NETWORK CALLBACKS

        // SPAWN jugadores cuando se conectan
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log("OnPlayerJoined. Player has join. Spawn player");
                NetworkPlayer newPlayer = runner.Spawn(playerPrefab, spawnPosition[spawnPositionIndex], Quaternion.identity, player);

                //Actualizmos lista y cantidad de jugadores en la sala
                _spawnedCharacters.Add(player, newPlayer.Object);

                //Añadimos el jugador a la lista de jugadores del TurnsManager
                TurnsManager.Instance.addPlayer(newPlayer);

                //Cambiamos la posición de respawn
                Debug.Log("SpawnIndex" + spawnPositionIndex);
                spawnPositionIndex++;

                //Asignamos materiales y turnos
                if (/*_spawnedCharacters.Count < 2*/ spawnPositionIndex < 2)
                {
                    newPlayer.RPC_setUpMaterials(true);
                }
                else
                {
                    newPlayer.RPC_setUpMaterials(false);
                }
            }
            else
            {

                Debug.Log("OnPlayerJoined");
            }


        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
               
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (characterInputHandler == null && NetworkPlayer.LocalPlayer != null)
                characterInputHandler = NetworkPlayer.LocalPlayer.GetComponent<CharacterInputHandler>();

            if (characterInputHandler != null)
                input.Set(characterInputHandler.GetNetworkInput());
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("OnConnectFailed " + reason);
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectedRequest");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            SceneManager.LoadScene(1);

            Debug.Log("OnDisconnectedFromServer");
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutDown");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            
        }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }

        #endregion

        #region FUNCTIONS

        #endregion

        #region GIZMOS

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPosition[0], radius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(spawnPosition[1], radius);
        }

        #endregion
    }
}

