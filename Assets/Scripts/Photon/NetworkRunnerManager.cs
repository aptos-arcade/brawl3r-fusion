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
        
        // network properties

        private NetworkRunner NetworkRunner { get; set; }
        
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
                NetworkRunner.SetActiveScene("MatchmakingScene");
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

        // callbacks

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            var playerInfo = MatchManager.PlayerInfos[player];
            playerInfo.IsActive = false;
            MatchManager.PlayerInfos.Set(player, playerInfo);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) {}

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            NetworkRunner = null;
            SceneManager.LoadScene("ModeSelectScene");
        }

        public void OnConnectedToServer(NetworkRunner runner) {}
        
        public void OnDisconnectedFromServer(NetworkRunner runner) {}

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}

        public void OnSceneLoadDone(NetworkRunner runner) {}

        public void OnSceneLoadStart(NetworkRunner runner) {}
    }
}