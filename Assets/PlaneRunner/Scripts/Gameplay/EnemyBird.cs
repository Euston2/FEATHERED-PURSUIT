using UnityEngine;

namespace Plane.Gameplay
{
    public class EnemyBird : MonoBehaviour
    {
        [Header("Chase Settings")]
        public float followSpeed = 1.5f;      // How fast it matches the player's movements
        public float restingZPosition = -30f; // Sits behind the plane (Player is at Z=0)

        [Header("Attack Settings")]
        public float attackCooldown = 5f;   // Seconds between attacks
        public float attackSpeed = 1f;     // How fast it lunges forward

        private float timer = 0f;
        private bool isAttacking = false;

        void Start()
        {
            // Start the bird in its correct position behind the player
            transform.position = new Vector3(0, 10, restingZPosition);
        }

        void Update()
        {
            // Don't do anything if the player is dead
            if (PlayerPlane.m_Main == null || !PlayerPlane.m_Main.gameObject.activeInHierarchy) return;

            if (!isAttacking)
            {
                // 1. STALK THE PLAYER
                // Calculate the target position: Match the player's X and Y, but stay back at restingZ
                Vector3 targetPos = new Vector3(
                    PlayerPlane.m_Main.transform.position.x,
                    PlayerPlane.m_Main.transform.position.y + 1.5f, // Hover slightly above them
                    restingZPosition
                );

                // Smoothly glide towards that target
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

                // 2. COUNTDOWN TO ATTACK
                timer += Time.deltaTime;
                if (timer >= attackCooldown)
                {
                    isAttacking = true;
                    timer = 0f;
                }
            }
            else
            {
                // 3. THE ATTACK!
                // Dash forward slowly, and limit the speed
                float moveStep = 8f * Time.deltaTime;
                transform.position += Vector3.forward * moveStep;

                // Force the reset even if it's struggling to reach Z=5
                if (transform.position.z > 2f) // Changed from 5f to 2f to reset sooner
                {
                    isAttacking = false;
                    Vector3 resetPos = transform.position;
                    resetPos.z = restingZPosition;
                    transform.position = resetPos;
                }
            }
        }
    }
}