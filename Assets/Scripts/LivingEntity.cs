using System;
using System.Collections;
using Datatypes;
using UnityEngine;
public class LivingEntity : MonoBehaviour
{
        public bool Waiting { get; set; }
        public bool Dead { get; set; }
        void Start()
        {
            Waiting = false;
        }

        protected void Die (CauseOfDeath cause) {
            if (!Dead) {
                Dead = true;
                Destroy (gameObject);
            }
        }
        
        public IEnumerator WaitForTime(int n)
        {
            //Print the time of when the function is first called.
            //Debug.Log("Started Coroutine at timestamp : " + Time.time);
            Waiting = true;

            //yield on a new YieldInstruction that waits for n seconds.
            yield return new WaitForSeconds(n);

            //After we have waited n seconds print the time again.
            //Debug.Log("Finished Coroutine at timestamp : " + Time.time);
            Waiting = false;
        }
    }
