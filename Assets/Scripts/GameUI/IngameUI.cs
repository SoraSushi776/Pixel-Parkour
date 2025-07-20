using GameCore;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class IngameUI : MonoBehaviour
    {
        public Text lifeDisplay;
        public Text scoreDisplay;
        public Text fpsDisplay;
        public Text timeDisplay;
        
        private void Start()
        {
            RefreshFpsDisplay();
        }
        
        private void Update()
        {
            lifeDisplay.text = $"x {LevelManager.CurrentLife}";
            scoreDisplay.text = $"Score {LevelManager.CurrentScore}";
            timeDisplay.text = $"Time {Mathf.RoundToInt(Player.LevelTime)}";
        }

        private void RefreshFpsDisplay()
        {
            fpsDisplay.text = $"FPS {Mathf.RoundToInt(1f / Time.deltaTime)}";
            
            Invoke(nameof(RefreshFpsDisplay), 1f);
        }
    }
}
