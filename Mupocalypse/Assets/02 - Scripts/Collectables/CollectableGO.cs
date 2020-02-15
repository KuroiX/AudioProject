using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
            
            if (collectable.showMessage)
            {
                /*var text = GameObject.FindGameObjectWithTag("Collect Message")?.GetComponent<Text>();
                if (text != null)
                    Player.Instance.StartCoroutine(TextUtil.Type(text, collectable.message, 0, 1));
                else
                    Debug.LogWarning("Collect Message not found.");*/
                GameObject.Find("PauseMenu").GetComponent<PauseMenu>().OpenMenu();
                GameObject.Find("PauseMenu").GetComponent<PauseMenu>().Abilites(true);
            }
            else Debug.LogWarning("Dont show");
            
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            foreach (GameObject go in doors)
            {
                go.GetComponent<Door>().UnLock();
            }
            
            Destroy(gameObject);
        }
    }
}
