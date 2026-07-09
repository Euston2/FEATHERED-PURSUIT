using UnityEngine;

namespace Plane.Gameplay
{
    public class EggItem : MonoBehaviour
    {
        public float pickupRadius = 2.5f; // How close the player needs to be to grab it

        void Update()
        {
            // 1. Move towards the player at the exact same speed as the road and obstacles
            transform.position += GameControl.m_Current.m_GameSpeed * Time.deltaTime * Vector3.back;

            // 2. Spin the egg so it looks like a shiny collectible
            transform.Rotate(0, 100 * Time.deltaTime, 0);

            // 3. Check if player grabbed it (using distance to avoid complex Unity Collider setups for now)
            if (PlayerPlane.m_Main != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, PlayerPlane.m_Main.transform.position);
                if (distanceToPlayer <= pickupRadius)
                {
                    // Tell the ScoreManager to add 1 point!
                    if (ScoreManager.instance != null)
                    {
                        ScoreManager.instance.AddScore(1);
                    }

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