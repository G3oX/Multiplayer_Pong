using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

namespace networking
{

    public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {

        #region Inspector Variables

        [Header("LOCAL")]
        [Space(1f)]
        [Header("SpawnPosition")]
        [SerializeField] Transform _spawnPosition;
        [SerializeField] float _SphereRadious = 1f;
        //[SerializeField] GameObject _hierarchySpawnerCointainer; // IMPLEMENTAR MÁS ADELANTE
        #endregion

        #region Networking Variables

        private NetworkRunner _runner;
        [Space(5f)]
        [Header("NETWORKING")]
        [Space(1f)]
        [Header("Player")]
        [SerializeField] private NetworkPrefabRef _playerPrefab;
        #endregion

        #region Private Variables

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        #endregion

        public async void StartGame(GameMode mode)
        {
            // Create the Fusion runner and let it know that we will be providing user input
            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Start or join (depends on gamemode) a session with a specific name
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
        }

        #region Networking CallBacks

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {

            // Create a unique position for the player
            //Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, _spawnPosition.position, Quaternion.identity, player);

            // Insert the new player's gameObject in the Players Container

            //networkPlayerObject.GetComponent<Transform>().SetParent(_hierarchySpawnerCointainer.transform);
            //networkPlayerObject.transform.SetParent(_hierarchySpawnerCointainer.transform);

            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {

            // Find and remove the players avatar
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }
        }
        public void OnInput(NetworkRunner runner, NetworkInput input) {

            var data = new NetworkInputData();

            if (Input.GetKey(KeyCode.LeftArrow))
                data.direction += Vector2.left;

            if (Input.GetKey(KeyCode.RightArrow))
                data.direction += Vector2.right;

            input.Set(data);
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }

        #endregion

        #region Local Methods

        public NetworkRunner getRunner()
        {
            return _runner;
        }
        #endregion

        #region DEBUG GIZMOS

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawWireSphere(_spawnPosition.position, _SphereRadious);
        }

        #endregion
    }
}

