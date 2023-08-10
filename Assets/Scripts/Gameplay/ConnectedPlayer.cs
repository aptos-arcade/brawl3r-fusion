using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class ConnectedPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerEliminations;
        [SerializeField] private TMP_Text playerDeaths;
        [SerializeField] private Image characterImage;

        public void SetPlayerInfo(string pName, Sprite image, int eliminations, int deaths, int team)
        {
            playerName.text = pName;
            characterImage.sprite = image;
            playerEliminations.text = eliminations.ToString();
            playerDeaths.text = deaths.ToString();
            playerName.color = team == MatchManager.Instance.LocalPlayerInfo.Team
                ? Color.white
                : Color.red;
        }
    }
}
