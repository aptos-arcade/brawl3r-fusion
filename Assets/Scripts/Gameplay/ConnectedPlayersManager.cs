using System.Collections.Generic;
using Photon;
using ScriptableObjects;
using UnityEngine;

namespace Gameplay
{
    public class ConnectedPlayersManager : MonoBehaviour
    {
        [SerializeField] private GameObject connectedPlayersPanel;
        [SerializeField] private GameObject connectedPlayersView;
        [SerializeField] private ConnectedPlayer connectedPlayerPrefab;
        [SerializeField] private CharacterImages characterImages;

        private void Update()
        {
            connectedPlayersPanel.SetActive(Input.GetKey(KeyCode.Tab));
        }
    
        public void ListAllPlayers(List<PlayerInfo> players)
        {
            // delete all children of connectedPlayersView but the first
            for (var i = 1; i < connectedPlayersView.transform.childCount; i++)
            {
                Destroy(connectedPlayersView.transform.GetChild(i).gameObject);
            }

            foreach (var player in players)
            {
                var roomPlayer = Instantiate(connectedPlayerPrefab, connectedPlayersView.transform);
                if(!player.IsActive) continue;
                roomPlayer.SetPlayerInfo(player.Name.ToString(), characterImages.GetCharacterSprite((int)player.Character),
                    player.Eliminations, 3 - player.Lives, player.Team);
            }
        }
    }
}
