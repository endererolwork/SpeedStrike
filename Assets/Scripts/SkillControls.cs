using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Race
{
    public class SkillControls : MonoBehaviour
    {
        [SerializeField]
        int maxMana = 5;

        int currentMana = 0;

        //------
        readonly int manaCostForExplosion = 3;
        public bool explosionActive = false;
        public bool scaleSkillReady = false;
        float scaleSkillCooldownCounter = 0;
        bool scaleIncrease = false;
        bool scaleDecrease = false;
        readonly float scaleSkillActiveTime = 2;
        float scaleSkillActiveTimeCounter = 0;
        readonly float scaleSkillCooldown = 10;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (scaleIncrease && scaleDecrease) { Debug.LogError("This really shouldnt happen"); }
            else if (scaleIncrease) 
            {
                transform.localScale += (Vector3.one * Time.deltaTime) / scaleSkillActiveTime;
                if (transform.localScale.x >= 2) 
                {
                    transform.localScale = Vector3.one * 2.0f;
                    scaleIncrease = false;
                    scaleDecrease = true;
                }
            }
            else if (scaleDecrease) 
            {
                transform.localScale -= (Vector3.one * Time.deltaTime) / scaleSkillActiveTime;
                if (transform.localScale.x <= 1)
                {
                    transform.localScale = Vector3.one;
                    scaleIncrease = false;
                    scaleDecrease = false;
                }
            }
            else
            {
                if (!scaleSkillReady)
                {
                    scaleSkillCooldownCounter += Time.deltaTime;
                }
            }

            if (scaleSkillCooldownCounter >= scaleSkillCooldown)
            {
                scaleSkillReady = true;
                scaleSkillCooldownCounter = 0;
            }


            if (Input.GetKeyDown(KeyCode.Q) && scaleSkillReady)
            {
                scaleSkillReady = false;
                scaleIncrease = true;
                scaleDecrease = false;
                // time based get bigger
            }
            if (Input.GetKeyDown(KeyCode.E) && currentMana >= manaCostForExplosion)
            {
                currentMana -= manaCostForExplosion;
                explosionActive = true;

                // mana used
            }



        }
    }
}
