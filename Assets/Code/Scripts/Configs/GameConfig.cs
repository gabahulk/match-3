using UnityEngine;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GameConfigs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public MatchType[] MatchTypes;
        public TileType[] TileTypes;
    }
}