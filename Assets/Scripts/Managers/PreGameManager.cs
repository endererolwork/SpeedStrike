using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameManager : MonoBehaviour
{

    int currentCarIndex;

    [SerializeField]
    GameObject BaseCar;

    [SerializeField]
    GameObject VirtualCameraObj;

    [SerializeField]
    GameObject[] DummyCarBodies;

    bool carSelected;
    public static PreGameManager Instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            if (DummyCarBodies.Length == 0)
            {
                Debug.LogError("No car list to select from");
            }
            if (BaseCar == null)
            {
                //Debug.LogError("No base car to derive from");
            }
            VirtualCameraObj.SetActive(false);
            BaseCar.GetComponent<Race.CarController>().enabled = false;
            currentCarIndex = 0;
            DummyCarBodies[currentCarIndex].SetActive(true);
            carSelected = false;
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!carSelected)
        { 
            // select the car
            if (Input.GetKeyDown(KeyCode.Return))
            {
                carSelected = true;
                BaseCar.GetComponent<Race.CarController>().enabled = true;
                BaseCar.GetComponent<CarShow>().enabled = false;
                VirtualCameraObj.SetActive(true);
                // some manager player set this car.
            }
            // next car
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                DummyCarBodies[currentCarIndex].SetActive(false);
                currentCarIndex++;
                if (currentCarIndex == DummyCarBodies.Length)
                {
                    currentCarIndex = 0;
                }
                DummyCarBodies[currentCarIndex].SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                DummyCarBodies[currentCarIndex].SetActive(false);
                currentCarIndex--;
                if (currentCarIndex == -1)
                {
                    currentCarIndex = DummyCarBodies.Length-1;
                }
                DummyCarBodies[currentCarIndex].SetActive(true);
            }
        }
    }
}
