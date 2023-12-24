using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Race
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        public int playerOneScore = 0;
        public int playerTwoScore = 0;

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
        
        }

        public void incrementPlayerScore(int playerNo = 0)
        {
            if (playerNo == 1)
            {
                playerOneScore++;
            }

            else if (playerNo == 2)
            {
                playerTwoScore++;
            }
            else 
            {
                Debug.LogWarning("NO SUCH PLAYER: " + playerNo.ToString());
            }

            return;
        }
    }
}
