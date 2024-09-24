using System;
using UnityEngine;

namespace Code.Scripts.Game.SoundPlayer
{
    [Serializable]
    public class SoundEntry
    {
        public AudioClip AudioClip;
        public string Name;
        [Range(0f, 1f)]
        public float Volume = 1;
    }
}