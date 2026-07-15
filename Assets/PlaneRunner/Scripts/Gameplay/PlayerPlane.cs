using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane.Gameplay
{
    public class PlayerPlane : MonoBehaviour
    {
        // 1. THIS IS THE NEW VARIABLE FOR YOUR JOYSTICK
        public Joystick movementJoystick;

        public Vector2 m_Angle = Vector2.zero;
        public Transform m_Base;
        Vector2 m_TurnSpeed = Vector2.zero;

        public GameObject m_ExplodeParticle;

        public static PlayerPlane m_Main;

        private void Awake()
        {
            m_Main = this;
        }

        void Start()
        {

        }

        void Update()
        {
            // 2. THIS IS THE NEW MOVEMENT LOGIC
            // Instead of checking for keyboard arrows, we just ask the joystick which way it is being pulled!
            float InputX = 0;
            float InputY = 0;

            // This ensures the game doesn't crash if the joystick isn't connected yet
            if (movementJoystick != null)
            {
                InputX = movementJoystick.Horizontal;
                InputY = movementJoystick.Vertical;
            }

            Vector3 movement = 40 * Time.deltaTime * new Vector3(InputX, InputY, 0);

            m_Angle.x = Mathf.Lerp(m_Angle.x, 60.0f * InputX, 5 * Time.deltaTime);
            m_Angle.y = Mathf.Lerp(m_Angle.y, 20.0f * InputY, 5 * Time.deltaTime);

            m_Base.localRotation = Quaternion.Euler(-1f * m_Angle.y, 0, -m_Angle.x);

            transform.position += movement;

            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, 8, 30);
            pos.x = Mathf.Clamp(pos.x, -18, 18);
            pos.z = 0;
            transform.position = pos;

            // --- YOUR COLLISION FIX REMAINS EXACTLY THE SAME ---
            Collider[] hits = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (Collider hit in hits)
            {
                // Ignore our own body
                if (hit.gameObject == gameObject)
                    continue;

                // Ignore ANYTHING that is not explicitly tagged "Enemy"
                if (!hit.gameObject.CompareTag("Enemy"))
                    continue;

                // If we get past the checks above, it means we hit an Enemy. Trigger Game Over!
                if (m_ExplodeParticle != null)
                {
                    GameObject obj = Instantiate(m_ExplodeParticle);
                    obj.transform.position = transform.position;
                }

                GameControl.m_Current.HandleGameOver();

                //  Turn off the joystick completely 
                if (movementJoystick != null)
                {
                    movementJoystick.gameObject.SetActive(false);
                }

                gameObject.SetActive(false);
                break;
                //GameControl.m_Current.HandleGameOver();
                //gameObject.SetActive(false);
                //break;
            }
        }
    }
}