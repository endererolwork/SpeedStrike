using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    float manaSphereSpawnTimer = 10;

    [SerializeField]
    Transform[] manaSphereSpawnPoints;

    [SerializeField]
    int maxManaSphereUp = 2;

    [SerializeField]
    GameObject ManaSpherePrefab;

    float manaSphereSpawnTimerCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (ManaSpherePrefab == null)
        {
            Debug.LogError("No Mana Sphere Prefab set in Game Manager.");
        }
        if (maxManaSphereUp > manaSphereSpawnPoints.Length)
        {
            Debug.LogError("max mana sphere cannot exceed the spawn point list size");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // im too lazy to mark this as editor only :D
        if (maxManaSphereUp > manaSphereSpawnPoints.Length)
        {
            Debug.LogError("max mana sphere cannot exceed the spawn point list size");
        }
        // editor only bitti xD

        if (maxManaSphereUp == GameObject.FindGameObjectsWithTag("ManaSphere").Length )
        {
            manaSphereSpawnTimerCounter = 0;
        }
        else if (manaSphereSpawnTimerCounter >= manaSphereSpawnTimer)
        {
            manaSphereSpawnTimerCounter = 0;
            // GameObject NewManaSphere = 
            int selectedPoint = Random.Range(0, manaSphereSpawnPoints.Length);
            while (manaSphereSpawnPoints[selectedPoint].childCount > 0)
            { 
                selectedPoint = Random.Range(0, manaSphereSpawnPoints.Length); 
            }
            Instantiate(ManaSpherePrefab, manaSphereSpawnPoints[selectedPoint]).transform.localPosition = Vector3.zero; ;

        }
        else
        {
            manaSphereSpawnTimerCounter += Time.deltaTime;
        }
    }
}
