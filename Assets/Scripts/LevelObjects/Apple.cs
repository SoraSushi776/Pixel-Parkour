using UnityEngine;

public class Apple : Collections
{
    // 吃苹果加速
    
    [SerializeField] private float bonusSpeed = 6f;
    [SerializeField] private float bonusSpeedHoldingTime = 3f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.AddSpeed(bonusSpeed, bonusSpeedHoldingTime);
        }
    }
}
