using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.Game.SoundPlayer
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<SoundEntry> entries = new ();


        private readonly Dictionary<string, SoundEntry> clips = new();
        private void Awake()
        {
            foreach (var entry in entries)
            {
                clips.Add(entry.Name, entry);
            }
            
            audioSource.clip = null;
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }

        private void PlaySound(string clipName)
        { 
            audioSource.clip = clips[clipName].AudioClip;
            audioSource.volume = clips[clipName].Volume;
            audioSource.Play();
        }

        public void OnDamageTaken()
        {
            PlaySound("damage");
        }
        
        public void OnAttack()
        {
            PlaySound("attack");
        }
        
        public void OnTilePopped()
        {
            PlaySound("tile_clear");
        }
    }
}
