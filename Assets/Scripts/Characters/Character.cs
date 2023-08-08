namespace Characters
{
    public enum CharactersEnum
    {
        None,
        PontemPirates, 
        AptosMonkeys, 
        Aptomingos, 
        BruhBears,
        DarkAges,
        Mavriks,
        Spooks,
    }
    
    public class Character
    {
        public string DisplayName { get; }
        
        public string PrefabName { get; }
        
        public string CollectionIdHash { get; }

        public Character(string displayName, string prefabName, string collectionIdHash) {
            DisplayName = displayName;
            PrefabName = prefabName;
            CollectionIdHash = collectionIdHash;
        }
    }
}