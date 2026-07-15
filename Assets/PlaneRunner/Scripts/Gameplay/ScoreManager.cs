using UnityEngine;
using TMPro; // Tells Unity we are using TextMeshPro!

namespace Plane.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        [SerializeField] TMP_Text scoreText;

        private float score = 0f;
        private float baseScorePerSecond = 100f;
        private int eggMultiplier = 1;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Update()
        {
            // --- THE FIX: Only count points if the player is alive and active on screen ---
            if (PlayerPlane.m_Main != null && PlayerPlane.m_Main.gameObject.activeInHierarchy)
            {
                score += (baseScorePerSecond * eggMultiplier) * Time.deltaTime;

                if (scoreText != null)
                {
                    scoreText.SetText("Score: " + Mathf.FloorToInt(score).ToString());
                }
            }
        }

        public void AddScore(int amount)
        {
            // Also prevents adding egg points if an egg is collected at the exact frame of death
            if (PlayerPlane.m_Main != null && PlayerPlane.m_Main.gameObject.activeInHierarchy)
            {
                score += (2000 * amount);
                eggMultiplier += amount;
            }
        }
    }
}