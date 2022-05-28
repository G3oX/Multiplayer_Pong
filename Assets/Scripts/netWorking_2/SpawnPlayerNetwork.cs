using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

namespace networking_2
{
    public class SpawnPlayerNetwork : MonoBehaviour, INetworkRunnerCallbacks
    {

        #region Inspector Variables

        [Header("LOCAL")]
        [Space(1f)]
        [Header("SpawnPosition")]
        [SerializeField] Transform _spawnPosition;
        [SerializeField] float _SphereRadious = 1f;
        //[SerializeField] GameObject _hierarchySpawnerCointainer; // IMPLEMENTAR MÁS ADELANTE

        [Space(3f)]
        [Header("NETWORKING")]
        [Space(1f)]
        [SerializeField] NetworkPlayer playerPrefab;

        #endregion

        PlayerInputHandler localInputHandler;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log("Hosting has joined");
                runner.Spawn(playerPrefab, _spawnPosition.position, Quaternion.identity, runner.LocalPlayer);
            }
            else Debug.Log("Another player has joined");

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if(localInputHandler == null && NetworkPlayer.Local != null)
            {
                localInputHandler = NetworkPlayer.Local.GetComponent<PlayerInputHandler>();
            }
            if(localInputHandler != null)
            {
                input.Set(localInputHandler.GetNetworkInput());
            }

        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutDown");
        }
        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("OnDisconnectedFromServer");
        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectRequest");
        }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("OnConnectFailed");
        }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }


        #region DEBUG GIZMOS

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(_spawnPosition.position, _SphereRadious);
        }

        #endregion
    }
}
