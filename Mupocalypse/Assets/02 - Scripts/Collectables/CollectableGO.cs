using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectableGO : MonoBehaviour
{
    [SerializeField]
    Collectable collectable;

    void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = collectable.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            collectable.ApplyEffect(other.GetComponent<Player>());
            ProgressManager.Instance.collectables.Add(collectable);
            Destroy(gameObject);
        }
    }
}
