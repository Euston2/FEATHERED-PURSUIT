using UnityEngine;

namespace Plane.Gameplay
{
    public class CliffEncounter : MonoBehaviour
    {
        public GameObject minionBirdPrefab;

        // This makes sure THIS specific cliff chunk only spawns birds once, 
        // otherwise it would spawn 60 birds per second while you fly through the box!
        private bool hasTriggered = false;

        void OnTriggerEnter(Collider other)
        {
            if (!hasTriggered && other.GetComponent<PlayerPlane>() != null)
            {
                hasTriggered = true;

                // 1. CLEANUP THE OLD WAVE
                // Find any existing minions from the previous cliff and destroy them
                // so we don't end up with an army of birds stacking behind you.
                MinionBird[] oldMinions = FindObjectsOfType<MinionBird>();
                foreach (MinionBird minion in oldMinions)
                {
                    Destroy(minion.gameObject);
                }

                // 2. THE NEW AMBUSH
                // Spawn Bird 1 far ahead, and tell it to flank to the Left (-6) later
                GameObject bird1 = Instantiate(minionBirdPrefab, new Vector3(-3f, 10f, 80f), Quaternion.identity);
                if (bird1.GetComponent<MinionBird>() != null)
                {
                    bird1.GetComponent<MinionBird>().offsetX = -6f;
                }

                // Spawn Bird 2 far ahead, and tell it to flank to the Right (+6) later
                GameObject bird2 = Instantiate(minionBirdPrefab, new Vector3(3f, 10f, 80f), Quaternion.identity);
                if (bird2.GetComponent<MinionBird>() != null)
                {
                    bird2.GetComponent<MinionBird>().offsetX = 6f;
                }
            }
        }
    }
}