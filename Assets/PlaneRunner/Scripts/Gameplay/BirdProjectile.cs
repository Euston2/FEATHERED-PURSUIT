using UnityEngine;

namespace Plane.Gameplay
{
    public class BirdProjectile : MonoBehaviour
    {
        public float projectileSpeed = 60f;

        void Update()
        {
            // Blast forward towards the player
            transform.position += Vector3.forward * projectileSpeed * Time.deltaTime;

            // Clean up the projectile if it completely misses and goes off screen
            if (transform.position.z > 20f)
            {
                Destroy(gameObject);
            }
        }
    }
}