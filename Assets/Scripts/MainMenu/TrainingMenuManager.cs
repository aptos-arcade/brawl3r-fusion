using Brawler;
using Fusion;
using Photon;
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
            continueButton.onClick.AddListener(JoinTrainingRoom);
        }

        private void JoinTrainingRoom()
        {
           NetworkRunnerManager.Instance.JoinRoom(HandleJoin, GameMode.Single, Global.GameModes.Training, 1, 1);
        }

        private void HandleJoin(bool success)
        {
            if (success) return;
            Debug.LogError("Failed to join room");
        }
    }
}
