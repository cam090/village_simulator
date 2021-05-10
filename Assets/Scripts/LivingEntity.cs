using System;
using System.Collections;
using Datatypes;
using UnityEngine;
public class LivingEntity : MonoBehaviour
{
        public bool waiting = false;
        private bool _dead;
        void Start()
        {
            
        }

        protected virtual void Die (CauseOfDeath cause) {
            if (!_dead) {
                _dead = true;
                Destroy (gameObject);
            }
        }
        
        public IEnumerator WaitForTime(int n)
        {
            //Print the time of when the function is first called.
            Debug.Log("Started Coroutine at timestamp : " + Time.time);
            waiting = true;

            //yield on a new YieldInstruction that waits for n seconds.
            yield return new WaitForSeconds(n);

            //After we have waited n seconds print the time again.
            Debug.Log("Finished Coroutine at timestamp : " + Time.time);
            waiting = false;
        }
    }
