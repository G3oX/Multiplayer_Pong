using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;


namespace Multiplayer
{
    public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
    {

        #region VARIABLES

        public NetworkPlayer playerPrefab;

        [Header("Spawn Point")]
        [SerializeField] Transform spawnPosition;
        [Tooltip("Radio del GIZMO que representa el punto de reaparición")]
        [SerializeField] float radius;

        //Otros Componentes
        CharacterInputHandler characterInputHandler;

        #endregion

        // Start is called before the first frame update
        void Start()
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
                Debug.Log("OnPlayerJoined. Host has join. Spawn player");
                runner.Spawn(playerPrefab, spawnPosition.position, Quaternion.identity, player);
            }
            else Debug.Log("OnPlayerJoined");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (characterInputHandler == null && NetworkPlayer.Local != null)
                characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

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

        #region GIZMOS

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPosition.position, radius);
        }

        #endregion
    }
}

