using DG.Tweening;
using UnityEngine;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(fileName = "New Match3BoardAnimationsConfig", menuName = "GameConfigs/BoardAnimationsConfig")]
    public class Match3BoardAnimationsConfig : ScriptableObject
    {
        public float ShakeDurations = .3f;
        public float ShakeStrength = 1f;
        public int ShakeVibrato = 10;
        public float ShakeRandomness = 90f;
        public bool ShakeSnapping = false;
        public bool ShakeFadeOut = true;
        public ShakeRandomnessMode ShakeRandomnessMode = ShakeRandomnessMode.Harmonic;

        public float ScaleTargetValue = 0.01f;
        public float ScaleDuration = .5f;
        public Color FadeColor = Color.clear;
        public float FadeDuration = .4f;
        
        public float TileFallDuration = .5f;
    }
}