using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs;

    [Range(0f, 1f)]
    public float chestChance;

    public void SpawnEnemies()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (Random.value <= chestChance)
            {
                Quaternion rot = spawnPoint.rotation * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

                GameObject go = Instantiate(
                    DungeonManager.Instance.ChestPrefab,
                    spawnPoint.position,
                    rot,
                    transform
                );

                Chest chest = go.GetComponent<Chest>();

                foreach (Item item in DungeonManager.Instance.ItemsToDrop)
                {
                    int minAmount = Random.Range(0, 4);
                    if(minAmount > 0)
                    {
                        chest.AddItem(item, minAmount, Random.Range(minAmount, 4), Random.Range(0f, 1f));
                    }
                }
            }
            else
            {
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                Quaternion rot = spawnPoint.rotation * Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Enemy enemy = Instantiate(prefab,
                                        spawnPoint.position,
                                        rot,
                                        transform)
                              .GetComponent<Enemy>();

                RoomController controller = GetComponent<RoomController>();
                controller.Enemies.Add(enemy);
                enemy.RoomController = controller;
            }
        }
    }
}
