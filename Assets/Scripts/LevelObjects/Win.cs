using GameCore;
using UnityEngine;

namespace LevelObjects
{
    public class Win : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.Instance.GameWin();
            }
        }
    }
}
