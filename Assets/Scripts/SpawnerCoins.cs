using Mirror;
using UnityEngine;

public class SpawnerCoins : NetworkBehaviour
{

    public GameObject MoneyPrefab;

    private void Start()
    {
        if (isServer)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                GameObject prefab = Instantiate(MoneyPrefab, pos, Quaternion.identity);
                NetworkServer.Spawn(prefab);
                prefab.GetComponent<SpriteRenderer>().enabled = true;

            }

        }
    }


}
