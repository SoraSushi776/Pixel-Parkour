using System;
using GameCore;
using UnityEngine;

namespace LevelObjects
{
    public class CheckPoint : MonoBehaviour
    {
        public AudioClip checkpointSound;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                LevelManager.LatestCheckPoint = transform;
                Player.PlayAudioClip(checkpointSound);
            }
        }
    }
}
