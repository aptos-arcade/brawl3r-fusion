using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Player.Commands;
using UnityEngine;
using Utilities;

namespace Player.NetworkBehaviours
{
    public class PlayerInputManager : NetworkBehaviour, INetworkRunnerCallbacks, IBeforeUpdate
    {
        
        [SerializeField] private PlayerController player;

        private float HorizontalInput { get; set; }
        
        private readonly List<Command> commands = new();

        public override void Spawned()
        {
            commands.Add(new JumpCommand(player, KeyCode.UpArrow));
            commands.Add(new DropCommand(player, KeyCode.DownArrow));
            commands.Add(new ShootCommand(player, KeyCode.Space));
            commands.Add(new JabMeleeCommand(player, KeyCode.S));
            commands.Add(new UpMeleeCommand(player, KeyCode.W));
            commands.Add(new SideMeleeCommand(player, KeyCode.A, -1));
            commands.Add(new SideMeleeCommand(player, KeyCode.D, 1));
            commands.Add(new ShieldCommand(player, KeyCode.LeftShift));
            commands.Add(new DashCommand(player, KeyCode.LeftArrow, -1));
            commands.Add(new DashCommand(player, KeyCode.RightArrow, 1));
            if(!FusionUtils.IsLocalPlayer(Object)) return;
            Runner.AddCallbacks(this);
        }
        
        public void BeforeUpdate()
        {
            if(!FusionUtils.IsLocalPlayer(Object) || !player.PlayerProperties.IsAcceptingInput) return;
            HorizontalInput = Input.GetAxisRaw("Horizontal");
        }

        public override void FixedUpdateNetwork()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (player.PlayerProperties.IsStunned || 
                !player.Runner.TryGetInputForPlayer<PlayerNetworkInput>(player.Object.InputAuthority, out var input))
                return;

            if (player.PlayerProperties.CanMove)
            {
                if (input.HorizontalInput != 0 || !player.PlayerProperties.IsDashing)
                {
                    player.PlayerNetworkState.Direction = new Vector2(input.HorizontalInput, 0);
                }
            }
            else if(!player.PlayerProperties.IsDodging)
            {
                player.PlayerNetworkState.Direction = Vector2.zero;
            }

            var pressed = input.NetworkButtons.GetPressed(player.PlayerNetworkState.PrevButtons);
            foreach (var command in commands)
            {
                if (pressed.WasPressed(player.PlayerNetworkState.PrevButtons, command.Button))
                {
                    command.WasPressed();
                }
                if(pressed.WasReleased(player.PlayerNetworkState.PrevButtons, command.Button))
                {
                    command.WasHeld();
                }
                else
                {
                    command.WasReleased();
                }
            }
            player.PlayerNetworkState.PrevButtons = input.NetworkButtons;
        }

        private PlayerNetworkInput GetPlayerInput()
        {
            var data = new PlayerNetworkInput
            {
                HorizontalInput = HorizontalInput,
            };
            foreach (var command in commands)
            {
                data.NetworkButtons.Set(command.Button, Input.GetKey(command.Key));
            }
            return data;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (runner == null || !runner.IsRunning) return;
            input.Set(GetPlayerInput());
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef) {}

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef) {}

        public void OnInputMissing(NetworkRunner runner, PlayerRef playerRef, NetworkInput input) {}

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}

        public void OnConnectedToServer(NetworkRunner networkRunner) {}

        public void OnDisconnectedFromServer(NetworkRunner runner) {}

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef playerRef, ArraySegment<byte> data) {}

        public void OnSceneLoadDone(NetworkRunner runner) {}

        public void OnSceneLoadStart(NetworkRunner runner) {}
    }
    
    public struct PlayerNetworkInput : INetworkInput
    {
        public float HorizontalInput;
        public NetworkButtons NetworkButtons;
    }
}
