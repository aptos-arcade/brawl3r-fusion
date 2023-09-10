using System.Collections.Generic;
using Fusion;
using Player.Commands;
using UnityEngine;

namespace Player.PlayerModules
{
    public class PlayerInputController
    {
        private readonly PlayerController player;

        private readonly List<Command> commands = new();

        public PlayerInputController(PlayerController player)
        {
            this.player = player;
            
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
        }

        public void HandleInput()
        {
            if (player.PlayerNetworkState.HurtTimer.IsRunning &&
                player.PlayerNetworkState.HurtTimer.Expired(player.Runner) && Input.anyKeyDown)
            {
                player.PlayerNetworkState.HurtTimer = TickTimer.None;
            }
            
            
            if (player.PlayerProperties.IsStunned) return;

            if (player.PlayerProperties.CanMove)
            {
                player.PlayerNetworkState.Direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            }
            else if(!player.PlayerProperties.IsDodging && !player.PlayerProperties.IsDashing)
            {
                player.PlayerNetworkState.Direction = Vector2.zero;
            }

            foreach (var command in commands)
            {
                if (Input.GetKeyDown(command.Key))
                {
                    command.GetKeyDown();
                }
                if (Input.GetKeyUp(command.Key))
                {
                    command.GetKeyUp();
                }
                if (Input.GetKey(command.Key))
                {
                    command.GetKey();
                }
            }
        }
    }
}
