using System;
using UnityEngine;
namespace ML_Agents.PF.Scripts.RL
{
    public class Block : MonoBehaviour
    {

        public bool IsPlayerIn;


        public void Init()
        {
            gameObject.tag = "Untagged";
        }

        public void ChangeToWall()
        {
            gameObject.tag = "wall";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "agent")
            {
                IsPlayerIn = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "agent")
            {

                IsPlayerIn = false;
            }
        }

    }
}
