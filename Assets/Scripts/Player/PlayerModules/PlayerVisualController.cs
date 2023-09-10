using System.Collections.Generic;
using Characters;
using Photon;
using UnityEngine;
using Utilities;

namespace Player.PlayerModules
{
    public class PlayerVisualController
    {
        private readonly PlayerController player;
        
        private List<SpriteRenderer> PlayerSprites { get; } = new();

        private List<Color> PlayerSpriteColors { get; } = new();
        
        public PlayerVisualController(PlayerController player)
        {
            this.player = player;
            var playerInfo = MatchManager.Instance.SessionPlayers[player.Object.InputAuthority];
            
            player.PlayerReferences.CharacterDisplay.SetCharacter(playerInfo.Character);
            player.PlayerReferences.GunSprite.SetGun(playerInfo.Gun);
            player.PlayerReferences.SwordSprite.SetSword(playerInfo.Sword);
            
            var tagColor = FusionUtils.IsSameTeam(player.Object)
                ? new Color(0.6588235f, 0.8078431f, 1f)
                : Color.red;
            player.PlayerReferences.NameTag.color = tagColor;
            player.PlayerReferences.CollectionTag.color = tagColor;
            player.PlayerReferences.NameTag.text = playerInfo.Name.ToString();
            player.PlayerReferences.CollectionTag.text =
                Characters.Characters.AvailableCharacters[playerInfo.Character].DisplayName;
            player.PlayerReferences.DamageDisplay.text = (player.PlayerNetworkState.DamageMultiplier - 1) * 100 + "%";

            AddSprites(playerInfo.Character);
        }
        
        private void AddSprites(CharactersEnum playerCharacter)
        {
            var playerTransform = player.PlayerReferences.CharacterDisplay.GetCharacter(playerCharacter).transform;
            foreach (Transform transform in playerTransform)
            {
                var sprite = transform.GetComponent<SpriteRenderer>();
                PlayerSprites.Add(sprite);
                PlayerSpriteColors.Add(sprite.color);
            }
        }
        
        public void HandleVisuals()
        {
            if(player.PlayerProperties.IsStunned) return;
            if (player.PlayerNetworkState.Direction.x != 0 && player.PlayerProperties.IsGrounded)
            {
                player.PlayerAnimations.TryWalk();
            }
            else if(player.PlayerComponents.RigidBody.velocity.magnitude < 0.1f && player.PlayerProperties.IsGrounded || 
                     Input.anyKey)
            {
                player.PlayerAnimations.TryIdle();
            }
            if(player.PlayerProperties.IsFalling)
            {
                player.PlayerAnimations.TryFall();
            }
        }

        public void ResetSpriteColors()
        {
            for (var i = 0; i < PlayerSprites.Count; i++)
            {
                PlayerSprites[i].color = PlayerSpriteColors[i];
            }
        }
        
        public void SetSpriteOpacity(float opacity)
        {
            foreach (var sprite in PlayerSprites)
            {
                var color = sprite.color;
                color.a = opacity;
                sprite.color = color;
            }
        }
        
        public void SetSpriteColors(Color color)
        {
            foreach (var sprite in PlayerSprites)
            {
                sprite.color = color;
            }
        }

        public void UpdateLivesDisplay()
        {
            var lives = MatchManager.Instance.SessionPlayers[player.Object.InputAuthority].Lives;
            for(var i = 0; i < player.PlayerReferences.PlayerLives.childCount; i++)
            {
                player.PlayerReferences.PlayerLives.GetChild(i).gameObject.SetActive(i < lives);
            }
        }
    }
}