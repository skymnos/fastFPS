using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;

public class SpawnPortal : MonoBehaviour
{
    [SerializeField] private float spawnRate;
    [SerializeField] private int NbOfEnemiesToSpawn;
    private int NbOfEnemiesSpawned;
    [SerializedDictionary("Enemy Type", "Probability To Spawn")]
    public SerializedDictionary<GameObject, float> spawnDitionary;
    private float timer;

    void Start()
    {
        timer = 0;
        NbOfEnemiesSpawned = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > spawnRate && NbOfEnemiesSpawned < NbOfEnemiesToSpawn) 
        {
            float minValue = 0;
            float maxValue = 0;
            float randomValue = Random.Range(0.0f, 1.0f);
            foreach (GameObject enemy in spawnDitionary.Keys)
            {
                maxValue += spawnDitionary[enemy];
                if ( minValue <= randomValue && randomValue < maxValue)
                {
                    Instantiate(enemy, transform.position, Quaternion.identity, transform);
                    break;
                }
                else
                {
                    minValue += spawnDitionary[enemy];
                    continue;
                }
            }
            timer = 0;
            NbOfEnemiesSpawned++;
        }
    }
}
