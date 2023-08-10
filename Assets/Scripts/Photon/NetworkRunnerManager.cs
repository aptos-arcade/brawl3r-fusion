using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon
{
    public class NetworkRunnerManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static NetworkRunnerManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public const string ModePropKey = "m";
        public const string NumTeamsPropKey = "nt";

        [SerializeField] private NetworkRunner networkRunnerPrefab;
        [SerializeField] private MatchManager matchManagerPrefab;
        
        // network properties
        
        public NetworkRunner NetworkRunner { get; private set; }
        
        public MatchManager MatchManager { get; set; }

        // events
        
        public async void JoinRoom(Action<bool> callback, GameMode photonGameMode, GameModes gameMode, int numTeams, 
            int numPlayers)
        {
            if (NetworkRunner == null)
            {
                NetworkRunner = Instantiate(networkRunnerPrefab);
            }
            NetworkRunner.AddCallbacks(this);
            NetworkRunner.ProvideInput = true;

            var startGameArgs = new StartGameArgs
            {
                GameMode = photonGameMode,
                SessionProperties = new Dictionary<string, SessionProperty>()
                {
                    { ModePropKey, (int)gameMode },
                    { NumTeamsPropKey, numTeams }
                },
                PlayerCount = numPlayers,
                SceneManager = NetworkRunner.GetComponent<INetworkSceneManager>(),
                CustomPhotonAppSettings = BuildCustomAppSetting(RegionManager.Instance.SelectedRegion.code)
                // ObjectPool = networkRunnerPrefab.GetComponent<ObjectPoolingManager>(),
            };
            var result = await NetworkRunner.StartGame(startGameArgs);
            if (result.Ok)
            {
                if (NetworkRunner.IsServer)
                {
                    NetworkRunner.Spawn(matchManagerPrefab, Vector3.zero, Quaternion.identity);
                }
                callback(true);
            }
            else
            {
                callback(false);
            }
        }
        
        public void LeaveRoom()
        {
            NetworkRunner.Shutdown();
        }
        
        private static AppSettings BuildCustomAppSetting(string region, string customAppID = null, 
            string appVersion = "1.0.0") {

            var appSettings = PhotonAppSettings.Instance.AppSettings.GetCopy();

            appSettings.UseNameServer = true;
            appSettings.AppVersion = appVersion;

            if (!string.IsNullOrEmpty(customAppID)) {
                appSettings.AppIdFusion = customAppID;
            }

            if (!string.IsNullOrEmpty(region)) {
                appSettings.FixedRegion = region.ToLower();
            }

            return appSettings;
        }

        private void OnDisconnect()
        {
            NetworkRunner = null;
            SceneManager.LoadScene("ModeSelectScene");
        }
        
        // callbacks

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player.PlayerId} joined");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (!runner.IsServer) return;
            var playerInfo = MatchManager.PlayerInfos[player];
            playerInfo.IsActive = false;
            MatchManager.PlayerInfos.Set(player, playerInfo);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {}

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.Log("Input missing");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (shutdownReason == ShutdownReason.HostMigration) {
                Debug.Log("Shutdown: Host Migration");
            } else {
                Debug.Log("Shutdown: Other");
                OnDisconnect();
            }
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("Connected to server with runner arg");
        }

        public void OnConnectedToServer()
        {
            Debug.Log("Connected to server without arg");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("Disconnected from server");
            OnDisconnect();
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("Connect request");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("Connect failed");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("User simulation message");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("Session list updated");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("Custom authentication response");
        }

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("Host migration");
            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
            NetworkRunner = null;

            // Step 2.2
            // Create a new Runner.
            NetworkRunner = Instantiate(networkRunnerPrefab);

            // setup the new runner...

            // Start the new Runner using the "HostMigrationToken" and pass a callback ref in "HostMigrationResume".
            var result = await NetworkRunner.StartGame(new StartGameArgs() {
                HostMigrationToken = hostMigrationToken,   // contains all necessary info to restart the Runner
                HostMigrationResume = HostMigrationResume, // this will be invoked to resume the simulation
                // other args
            });

            // Check StartGameResult as usual
            if (result.Ok == false) {
                Debug.LogWarning(result.ShutdownReason);
            } else {
                Debug.Log("Done");
            }
        }

        private static void HostMigrationResume(NetworkRunner runner) {

            // Get a temporary reference for each NO from the old Host
            foreach (var resumeNo in runner.GetResumeSnapshotNetworkObjects())

                if (
                    // Extract any NetworkBehavior used to represent the position/rotation of the NetworkObject
                    // this can be either a NetworkTransform or a NetworkRigidBody, for example
                    resumeNo.TryGetBehaviour<NetworkPosition>(out var posRot)) {

                    runner.Spawn(resumeNo, position: posRot.ReadPosition(), rotation: Quaternion.identity, onBeforeSpawned: (_, newNo) =>
                    {
                        // One key aspects of the Host Migration is to have a simple way of restoring the old NetworkObjects state
                        // If all state of the old NetworkObject is all what is necessary, just call the NetworkObject.CopyStateFrom
                        newNo.CopyStateFrom(resumeNo);

                        // and/or

                        // If only partial State is necessary, it is possible to copy it only from specific NetworkBehaviours
                        if (resumeNo.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                        {
                            newNo.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                        }
                    });
                }
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            Debug.Log("Reliable data received");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("Scene load done");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("Scene load start");
        }
    }
}