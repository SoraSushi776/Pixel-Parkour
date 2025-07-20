using GameCore;
using UnityEngine;

namespace LevelObjects
{
    public class Apple : MonoBehaviour
    {
        // 吃苹果加速
        [SerializeField] private string collectionName = "Apple";
        [SerializeField] private int score = 100;
        [SerializeField] private float bonusSpeed = 6f;
        [SerializeField] private float bonusSpeedHoldingTime = 3f;
        [SerializeField] private bool destroyAfterEnter;
        [SerializeField] private AudioClip eatSound;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                LevelManager.CurrentScore += score;
                
                Player player = other.GetComponent<Player>();
                player.AddSpeed(bonusSpeed, bonusSpeedHoldingTime);
            
                if (destroyAfterEnter)
                    Destroy(gameObject);
                
                Player.PlayAudioClip(eatSound);
            }
        }
    }
}
