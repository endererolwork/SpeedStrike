using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Race
{
    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager Instance { get; private set; }

        private int[] playerHitCount = {0, 0};
        private float[] playerControlledTime = { 0.0f, 0.0f };
        private float[] playerYariSahaTime = { 0.0f, 0.0f };

        private int ballOwner = 0;
        private int ballLocation = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (ballOwner == 1 || ballOwner == 2)
            {
                playerControlledTime[ballOwner - 1] += Time.deltaTime;
            }
            if (ballLocation == 1 || ballLocation == 2)
            {
                playerYariSahaTime[ballLocation - 1] += Time.deltaTime;
            }

        }

        public void switchToPlayer(int playerNo)
        {
            if (playerNo != 1 || playerNo != 2)
            {
                Debug.LogWarning("Unknown player.");
                return;
            }
            ballOwner = playerNo;
            playerHitCount[playerNo - 1] ++;
        }

    }
}
