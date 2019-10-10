using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapSpawner : MonoBehaviour
{
    [SerializeField] public List<Rigidbody2D> scraps = new List<Rigidbody2D>();
    [SerializeField] public float maxFlingForce = 5f;
    [SerializeField] public float minFlingForce = 2f;
    [SerializeField] public float minScrapLifetime = 3.5f;
    [SerializeField] public float maxScrapLifetime = 4.5f;
    [SerializeField] public float minRotForce = -15f;
    [SerializeField] public float maxRotForce = 15f;
    [SerializeField] public float randomSpawnRadius = 1.0f;

    public void Spawn() {
        for (int i = 0; i < scraps.Count; i++) {
            scraps[i].gameObject.SetActive(true);
            scraps[i].transform.localPosition = Random.insideUnitCircle * randomSpawnRadius;

            Vector2 direction = new Vector2(Random.Range(-1f, 1f), Random.Range(0f, 0.5f)).normalized;
            scraps[i].AddForce(direction * Random.Range(minFlingForce, maxFlingForce));
            scraps[i].MoveRotation(Random.Range(minRotForce, maxRotForce));

            Destroy(scraps[i].gameObject, Random.Range(minScrapLifetime, maxScrapLifetime));
        }
        Destroy(gameObject, maxScrapLifetime + 0.1f);
    }
}
