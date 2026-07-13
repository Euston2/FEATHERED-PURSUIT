using UnityEngine;

namespace Plane.Gameplay
{
    public class EggItem : MonoBehaviour
    {
        [Header("Pickup Settings")]
        public float pickupRadius = 2.5f;

        [Header("Weight Mechanic")]
        public float weightPenalty = 5f; // How much speed is lost per egg
        public float minimumSpeed = 10f; // The absolute slowest the bird can go

        void Update()
        {
            // 1. THE TREADMILL FIX
            // Move towards the player at the exact same speed as the road
            transform.position += GameControl.m_Current.m_GameSpeed * Time.deltaTime * Vector3.back;

            // 2. Spin the egg so it looks like a shiny collectible
            transform.Rotate(0, 100 * Time.deltaTime, 0);

            // 3. Check if player grabbed it
            if (PlayerPlane.m_Main != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, PlayerPlane.m_Main.transform.position);

                if (distanceToPlayer <= pickupRadius)
                {
                    // Add to the score
                    if (ScoreManager.instance != null)
                    {
                        ScoreManager.instance.AddScore(1);
                    }

                    // Apply the weight mechanic (Slow down the global game speed)
                    if (GameControl.m_Current != null)
                    {
                        GameControl.m_Current.m_GameSpeed -= weightPenalty;

                        // Clamp the speed so the game doesn't completely stop or go in reverse
                        if (GameControl.m_Current.m_GameSpeed < minimumSpeed)
                        {
                            GameControl.m_Current.m_GameSpeed = minimumSpeed;
                        }
                    }

                    // Destroy the egg after collection
                    Destroy(gameObject);
                }
            }

            // 4. Destroy the egg if the player misses it and it goes off screen
            if (transform.position.z < -50)
            {
                Destroy(gameObject);
            }
        }
    }
}