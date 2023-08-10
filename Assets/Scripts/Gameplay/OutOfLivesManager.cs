using Photon;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class OutOfLivesManager : MonoBehaviour
    {

        [SerializeField] private GameObject outOfLivesUI;
        [SerializeField] private Button watchGameButton;
        [SerializeField] private Button exitButton;

        private void Start()
        {
            watchGameButton.onClick.AddListener(() => SetOutOfLivesUI(false));
            exitButton.onClick.AddListener(NetworkRunnerManager.Instance.LeaveRoom);
        }
        
        public void SetOutOfLivesUI(bool active)
        {
            outOfLivesUI.SetActive(active);
        }
    }
}