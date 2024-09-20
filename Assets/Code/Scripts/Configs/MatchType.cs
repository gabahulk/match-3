using Code.Scripts.Match3;
using UnityEngine;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(fileName = "New MatchType", menuName = "GameConfigs/MatchType")]
    public class MatchType : ScriptableObject
    {
            [Multiline] public string matchFormat;
            
            private Shape GetMatchShape()
            {
                var lines = matchFormat.Split('\n');
                var height = lines.Length;
                var width = lines[0].Split(',').Length;
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

                return new Shape(result);
            }

            public Match IsMatch(TileCluster cluster)
            {
                return ShapeMatchValidator.CheckIfShapeAContainsShapeB(cluster.Shape, GetMatchShape());
            }
    }
}