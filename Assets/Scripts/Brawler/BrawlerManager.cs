using System;
using Characters;
using UnityEngine;

namespace Brawler
{
    public class BrawlerManager : MonoBehaviour
    {
        public static BrawlerManager Instance { get; private set; }

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
        
        public Brawler Brawler { get; } = new();
        
        public static event Action OnBrawlerChanged;
        
        public void SetBrawlerCharacter(CharactersEnum character)
        {
            Brawler.Character = character;
            OnBrawlerChanged?.Invoke();
        }
        
        public void SetBrawlerGun(int gunIndex)
        {
            Brawler.Gun = (Guns)gunIndex;
            OnBrawlerChanged?.Invoke();
        }

        public void SetBrawlerSword(int swordIndex)
        {
            Brawler.Sword = (Swords)swordIndex;
            OnBrawlerChanged?.Invoke();
        }
        
        public void SetName(string brawlerName)
        {
            Brawler.Name = brawlerName;
            OnBrawlerChanged?.Invoke();
        }
        
        public void SetId(string id)
        {
            Brawler.Id = id;
            OnBrawlerChanged?.Invoke();
        }

    }
}