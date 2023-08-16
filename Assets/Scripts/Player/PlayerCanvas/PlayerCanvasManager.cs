using Fusion;
using Gameplay;
using UnityEngine;
using Utilities;

namespace Player.PlayerCanvas
{
    public class PlayerCanvasManager : SimulationBehaviour
    {
        [SerializeField] private EnergyManager energyManager;
        [SerializeField] private KillTextManager killTextManager;
        
        public void ShowEnergyUi(bool active)
        {
            if (!FusionUtils.IsLocalPlayer(Object)) return;
            energyManager.gameObject.SetActive(active);
        }
        
        public void OnKill()
        {
            if (!FusionUtils.IsLocalPlayer(Object)) return;
            killTextManager.OnKill();
        }
        
        public void NoEnergy(EnergyManager.EnergyType energyType)
        {
            if (!FusionUtils.IsLocalPlayer(Object)) return;
            energyManager.NoEnergy(energyType);
        }
    }
}