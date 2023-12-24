using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Race
{
    public class GateChecker : MonoBehaviour
    {
        [SerializeField]
        private int playerNo = 0;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("top"))
            {
                ScoreManager.Instance.incrementPlayerScore(playerNo);
            }
            return;
        }
    }
}
