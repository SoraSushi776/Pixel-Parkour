using GameCore;
using UnityEngine;

namespace LevelObjects
{
    public class Fire : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.Instance.Death();
            }
        }
    }
}
