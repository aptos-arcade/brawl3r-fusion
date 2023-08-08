using UnityEngine;

namespace Characters
{
    public class CharacterDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject[] characters;
        
        public void SetCharacter(CharactersEnum character)
        {
            foreach (var characterObject in characters)
            {
                characterObject.SetActive(false);
            }
            
            characters[(int)character].SetActive(true);
        }
    }
}