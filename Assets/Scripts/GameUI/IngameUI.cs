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
        
        public Text gameWinDisplay;
        public Text gameOverDisplay;
        
        private void Start()
        {
            RefreshFpsDisplay();
            
            Player.OnGameWin.AddListener(() => gameWinDisplay.gameObject.SetActive(true));
            Player.OnGameOver.AddListener(() => gameOverDisplay.gameObject.SetActive(true));
        }
        
        private void Update()
        {
            lifeDisplay.text = $"x {LevelManager.CurrentLife}";
            scoreDisplay.text = $"Score {LevelManager.CurrentScore}";
            timeDisplay.text = $"Time {(Mathf.RoundToInt(Player.LevelTime) > 0 ? Mathf.RoundToInt(Player.LevelTime) : 0)}";
        }

        private void RefreshFpsDisplay()
        {
            fpsDisplay.text = $"FPS {Mathf.RoundToInt(1f / Time.deltaTime)}";
            
            Invoke(nameof(RefreshFpsDisplay), 1f);
        }
    }
}
