using Code.Scripts.SOArchitecture;
using UnityEngine;

namespace Code.Scripts.Game.Enemy
{
    [CreateAssetMenu (fileName = "New Enemy Set", menuName = "Code/Runtime Sets/EnemySet")]
    public class EnemyRuntimeSet : RuntimeSet<Enemy> {}
}