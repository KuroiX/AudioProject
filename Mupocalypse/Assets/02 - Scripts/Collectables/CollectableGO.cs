using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollectableGO : MonoBehaviour
{
    [SerializeField]
    Collectable collectable;

    public int id {private get; set;}

    void Start()
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = collectable.sprite;
        sr.color = collectable.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = ProgressManager.Instance;
            collectable.ApplyEffect(other.GetComponent<Player>());
            pm.collectables.Add(collectable);
            pm.MarkDestroyed(id);
            Destroy(gameObject);
        }
    }
}
