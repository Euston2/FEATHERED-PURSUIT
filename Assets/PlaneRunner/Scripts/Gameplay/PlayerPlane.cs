using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane.Gameplay
{
    public class PlayerPlane : MonoBehaviour
    {
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
            float InputX = 0, InputY = 0;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                InputX = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                InputX = 1;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                InputY = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                InputY = -1;
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

            // --- THE FIX IS HERE ---
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
                gameObject.SetActive(false);
                break;
            }
        }
    }
}