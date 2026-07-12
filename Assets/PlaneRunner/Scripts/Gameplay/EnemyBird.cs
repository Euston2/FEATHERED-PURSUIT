using UnityEngine;

namespace Plane.Gameplay
{
    public class EnemyBird : MonoBehaviour
    {
        [Header("Chase Settings")]
        public float chaseSpeed = 10f;         // Real max speed (units/sec) — bird can be outrun if you're faster
        public float restingZPosition = -30f;
        public float hoverOffsetY = 3f;        // Bird hovers above you, not exactly on your Y
        public float hoverOffsetX = 2f;        // Bird hovers to a side, not exactly on your X

        [Header("Attack Settings")]
        public float attackCooldown = 8f;
        public float dashSpeed = 25f;
        public float homingStrength = 4f;

        [Header("Wind-Up (Telegraph)")]
        public float windUpDuration = 0.6f;

        private float timer = 0f;
        private bool isAttacking = false;
        private bool isWindingUp = false;
        private float windUpTimer = 0f;

        void Start()
        {
            transform.position = new Vector3(0, 10, restingZPosition);
        }

        void Update()
        {
            if (PlayerPlane.m_Main == null || !PlayerPlane.m_Main.gameObject.activeInHierarchy) return;

            Vector3 playerPos = PlayerPlane.m_Main.transform.position;

            if (isWindingUp)
            {
                // --- WIND-UP PHASE ---
                windUpTimer += Time.deltaTime;

                if (windUpTimer >= windUpDuration)
                {
                    isWindingUp = false;
                    isAttacking = true;
                }
            }
            else if (!isAttacking)
            {
                // --- STALKING PHASE ---
                // Target a point near you, not your exact X/Y, so the bird never overlaps your position
                Vector3 targetPos = new Vector3(playerPos.x + hoverOffsetX, playerPos.y + hoverOffsetY, restingZPosition);

                // Real max speed — if you move faster than chaseSpeed, a visible gap opens up.
                // This is the key fix: MoveTowards caps the speed, unlike Lerp which always closes distance proportionally.
                transform.position = Vector3.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

                timer += Time.deltaTime;
                if (timer >= attackCooldown)
                {
                    isWindingUp = true;
                    windUpTimer = 0f;
                    timer = 0f;
                }
            }
            else
            {
                // --- ATTACK DASH ---
                float newZ = transform.position.z + (dashSpeed * Time.deltaTime);
                float newX = Mathf.Lerp(transform.position.x, playerPos.x, homingStrength * Time.deltaTime);
                float newY = Mathf.Lerp(transform.position.y, playerPos.y, homingStrength * Time.deltaTime);

                transform.position = new Vector3(newX, newY, newZ);

                // --- RELOAD ---
                if (transform.position.z > 2f)
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