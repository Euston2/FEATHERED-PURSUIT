using UnityEngine;
using TMPro; // Tells Unity we are using TextMeshPro!

namespace Plane.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        // Changed 'Text' to 'TextMeshProUGUI'
        public TextMeshProUGUI scoreText;
        private int currentScore = 0;

        void Awake()
        {
            instance = this;
            currentScore = 0;
        }

        public void AddScore(int points)
        {
            currentScore += points;

            if (scoreText != null)
            {
                scoreText.text = "Eggs: " + currentScore;
            }
        }
    }
}