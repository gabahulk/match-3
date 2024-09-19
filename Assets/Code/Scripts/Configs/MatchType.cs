using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(fileName = "New MatchType", menuName = "GameConfigs/MatchType")]
    public class MatchType : ScriptableObject
    {
            [Multiline] public string matchFormat;

            public int[,] GetMatchFormat()
            {
                var lines = matchFormat.Split('\n');
                var height = lines.Length;
                var width = lines[0].Length;
                var result = new int[height, width];
                for (var i = 0; i < height; i++)
                {
                    var line = lines[i];
                    var values = line.Split(',');
                    for (int j = 0; j < width; j++)
                    {
                        result[i, j] = int.Parse(values[j]);
                    }
                }

                return result;
            }
    }
}