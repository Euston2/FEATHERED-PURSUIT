using UnityEngine;

namespace Plane.Gameplay
{
    public class MinionBird : MonoBehaviour
    {
        [Header("Phase 1: Front Attack")]
        public float diveSpeed = 50f;

        [Header("Phase 2: Joining the Boss")]
        public float followSpeed = 5f;
        public float restingZPosition = -25f; // Behind the player
        public float offsetX = -5f; // Used to put one bird on the left, one on the right

        [Header("Phase 3: Shooting")]
        public GameObject projectilePrefab;
        public float shootCooldown = 2f;

        private float timer = 0f;

        // This keeps track of what the bird should be doing right now
        private enum Phase { Diving, Repositioning, Shooting }
        private Phase currentPhase = Phase.Diving;

        void Update()
        {
            if (PlayerPlane.m_Main == null) return;
            Vector3 playerPos = PlayerPlane.m_Main.transform.position;

            if (currentPhase == Phase.Diving)
            {
                // Blast backwards from the front of the screen towards the player
                transform.position += Vector3.back * diveSpeed * Time.deltaTime;

                // If they miss and fly past the player, switch to Reposition mode
                if (transform.position.z < playerPos.z - 5f)
                {
                    currentPhase = Phase.Repositioning;
                }
            }
            else if (currentPhase == Phase.Repositioning)
            {
                // Swoop around to the back to form a V-formation with the main enemy
                Vector3 targetPos = new Vector3(playerPos.x + offsetX, playerPos.y, restingZPosition);
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

                // Once they reach their flanking position, lock in and start shooting
                if (Vector3.Distance(transform.position, targetPos) < 2f)
                {
                    currentPhase = Phase.Shooting;
                }
            }
            else if (currentPhase == Phase.Shooting)
            {
                // Hover continuously in the V-formation
                Vector3 targetPos = new Vector3(playerPos.x + offsetX, playerPos.y, restingZPosition);
                transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

                // Start firing the projectiles forward!
                timer += Time.deltaTime;
                if (timer >= shootCooldown)
                {
                    if (projectilePrefab != null)
                    {
                        Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    }
                    timer = 0f;
                }
            }
        }
    }
}