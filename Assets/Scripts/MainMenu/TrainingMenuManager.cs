using Brawler;
using Fusion;
using Photon;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class TrainingMenuManager : MonoBehaviour
    {
        [SerializeField] private Button continueButton;

        private void Start()
        {
            BrawlerManager.Instance.SetBrawlerCharacter(Characters.Characters.GetRandomCharacter());
            BrawlerManager.Instance.SetBrawlerGun(Random.Range(1, 6));
            BrawlerManager.Instance.SetBrawlerSword(Random.Range(1, 6));
            BrawlerManager.Instance.SetName(AuthenticationService.Instance.PlayerName.Split("#")[0]);
            continueButton.onClick.AddListener(JoinTrainingRoom);
        }

        private static void JoinTrainingRoom()
        {
           NetworkRunnerManager.Instance.JoinRoom(HandleJoin, GameMode.Host, 
               Global.GameModes.Training, 1, 1);
        }

        private static void HandleJoin(bool success)
        {
            if (success) return;
            Debug.LogError("Failed to join room");
        }
    }
}
