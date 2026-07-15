using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plane.UI;

namespace Plane.Gameplay
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl m_Current;

        [HideInInspector]
        public int m_GameState = 0;
        public const int State_Start = 0;
        public const int State_Chase = 1;
        public const int State_Shoot = 2;
        public const int State_Win = 3;
        public const int State_Lose = 4;
        [HideInInspector]
        public float State_Timer = 0;

        public Transform m_SpeedParticle;

        public float m_GameSpeed = 100;

        // --- 1. NEW STUN VARIABLES ---
        public static bool enemiesStunned = false;
        private bool isStunOnCooldown = false;

        public UnityEngine.UI.Button stunUIObject;

        void Awake()
        {
            m_Current = this;
        }

        void Start()
        {
            m_GameState = State_Start;
        }

        void Update()
        {
            State_Timer += Time.deltaTime;
        }

        // --- 2. NEW STUN LOGIC ---
        // This is the function your UI Button will call
        public void ActivateStun()
        {
            if (!isStunOnCooldown)
            {
                StartCoroutine(StunRoutine());
            }
        }

        // This handles the timer so it doesn't freeze your game
        private IEnumerator StunRoutine()
        {
            isStunOnCooldown = true;
            enemiesStunned = true; // Flips the switch to freeze the birds!

            // Wait for 5 seconds of stun time
            yield return new WaitForSeconds(5f);

            enemiesStunned = false; // Unfreeze the birds!

            // Add a 10-second cooldown before the button works again
            yield return new WaitForSeconds(10f);
            isStunOnCooldown = false;

            if (stunUIObject != null)
            {
                stunUIObject.interactable = true; // Re-enable the button after cooldown
            }
        }

        public void HandleGameOver()
        {
            m_GameSpeed = 0;
            m_SpeedParticle.gameObject.SetActive(false);
            CameraControl.Current.m_ShakeEnabled = false;
            UIControl.Current.m_InGameUI.SetActive(false);
            UIControl.Current.m_LoseUI.SetActive(true);
        }

        public void HandleWin()
        {

        }
    }
}