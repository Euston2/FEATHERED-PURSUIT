using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane.Gameplay
{
    [System.Serializable]
    public class EnvironmentTheme
    {
        public string ThemeName;
        public GameObject[] RoadPrefabs;

        [Header("Environment Specifics")]
        public GameObject[] ObstaclePrefabs;
    }

    public class RoadCreator : MonoBehaviour
    {
        public static RoadCreator m_Current;

        [Header("Environment Settings")]
        public EnvironmentTheme[] m_Environments;
        public int m_SegmentsPerEnvironment = 20;

        [Header("Universal Prefabs")]
        public GameObject[] m_ItemPackPrefabs;

        [HideInInspector] public RoadPart m_LastPart;
        [HideInInspector] public ObstaclePack m_LastObstacle;
        [HideInInspector] public GameObject m_LastItem; // Added this to track the eggs
        [HideInInspector] public List<ObstaclePack> m_Obstacles;

        [HideInInspector] public int ObstacleCounter = 0;
        [HideInInspector] public int ItemCounter = 0;

        private int m_CurrentThemeIndex = 0;
        private int m_SegmentsSpawnedInCurrentTheme = 0;

        void Awake()
        {
            m_Current = this;
        }

        void Start()
        {
            m_Obstacles = new List<ObstaclePack>();
            RoadPart last = null;

            for (int i = 0; i < 10; i++)
            {
                last = SpawnRoadSegment(last, i == 0);
            }
            m_LastPart = last;

            SpawnObstacle(500f);
            SpawnItem(300f); // NEW: Spawns the very first egg when the game starts!
        }

        void Update()
        {
            // 1. Spawn Roads
            if (m_LastPart != null && m_LastPart.transform.position.z < 200)
            {
                for (int i = 0; i < 10; i++)
                {
                    m_LastPart = SpawnRoadSegment(m_LastPart, false);
                }
            }

            // 2. Spawn Obstacles
            if (m_LastObstacle != null && m_LastObstacle.transform.position.z < 200)
            {
                m_LastObstacle = null;
                // Notice I changed 400f to m_LastPart.transform.position.z to fix that environment bug!
                SpawnObstacle(m_LastPart.transform.position.z);
            }

            // 3. NEW: Spawn Eggs (Items)
            if (m_LastItem != null && m_LastItem.transform.position.z < 200)
            {
                m_LastItem = null;
                // Spawns the egg midway between the obstacles
                SpawnItem(m_LastPart.transform.position.z + 200f);
            }
        }

        private RoadPart SpawnRoadSegment(RoadPart previousPart, bool isFirstPiece)
        {
            if (m_SegmentsSpawnedInCurrentTheme >= m_SegmentsPerEnvironment)
            {
                m_SegmentsSpawnedInCurrentTheme = 0;
                m_CurrentThemeIndex++;

                if (m_CurrentThemeIndex >= m_Environments.Length)
                {
                    m_CurrentThemeIndex = 0;
                }
            }

            EnvironmentTheme currentTheme = m_Environments[m_CurrentThemeIndex];
            int randomPiece = Random.Range(0, currentTheme.RoadPrefabs.Length);

            if (isFirstPiece) randomPiece = 0;

            GameObject obj = Instantiate(currentTheme.RoadPrefabs[randomPiece]);
            RoadPart newPart = obj.GetComponent<RoadPart>();

            if (isFirstPiece)
            {
                obj.transform.position = Vector3.zero;
            }
            else if (previousPart != null)
            {
                obj.transform.position = previousPart.EndPoint.position;
                previousPart.m_NextPart = newPart;
            }

            m_SegmentsSpawnedInCurrentTheme++;
            return newPart;
        }

        private void SpawnObstacle(float zPosition)
        {
            EnvironmentTheme currentTheme = m_Environments[m_CurrentThemeIndex];

            if (currentTheme.ObstaclePrefabs == null || currentTheme.ObstaclePrefabs.Length == 0) return;

            int r = Random.Range(0, currentTheme.ObstaclePrefabs.Length);
            GameObject prefabToSpawn = currentTheme.ObstaclePrefabs[r];

            GameObject obj = Instantiate(prefabToSpawn, new Vector3(0, 0, zPosition), Quaternion.identity);

            ObstaclePack packScript = obj.GetComponent<ObstaclePack>();

            if (packScript == null)
            {
                packScript = obj.AddComponent<ObstaclePack>();
                Debug.Log("Auto-Attached missing script to: " + prefabToSpawn.name);
            }

            m_LastObstacle = packScript;
        }

        // Handles spawning the eggs in dense clusters!
        private void SpawnItem(float zPosition)
        {
            // Safety check: Don't spawn if the list is empty
            if (m_ItemPackPrefabs == null || m_ItemPackPrefabs.Length == 0) return;

            // Pick the egg prefab from your list
            int r = Random.Range(0, m_ItemPackPrefabs.Length);
            GameObject prefabToSpawn = m_ItemPackPrefabs[r];

            // --- THE DENSITY SETTINGS ---
            int eggsInCluster = 5; // How many eggs spawn in a row
            float spacing = 4f;    // The distance between each egg in the line

            // This loop spawns multiple eggs to create a dense line
            for (int i = 0; i < eggsInCluster; i++)
            {
                // Calculate the position for this specific egg in the line
                float currentZPosition = zPosition + (i * spacing);
                Vector3 spawnPosition = new Vector3(0, 10f, currentZPosition); // Y=2 keeps it above ground

                // Spawn the egg
                GameObject obj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

                // Auto-attach the script if you forgot it
                EggItem itemScript = obj.GetComponent<EggItem>();
                if (itemScript == null)
                {
                    itemScript = obj.AddComponent<EggItem>();
                }

                // We only want to track the VERY LAST egg in the cluster 
                // so the game knows when to spawn the next group!
                if (i == eggsInCluster - 1)
                {
                    m_LastItem = obj;
                }
            }
        }
    }
}