using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Vector2 innerBounds = new Vector2(1, 1);
    [SerializeField]
    Vector2 innerBoundsOffset = new Vector2(0, 0);
    [SerializeField]
    Transform player = null;
    [SerializeField]
    float followSpeed = 1;

    public Vector2 min {private get; set;}
    public Vector2 max {private get; set;}

    Rect innerBoundsRect;

    private void Start() {
        // Test
        // min = new Vector2(-1, -1);
        // max = new Vector2(1, 1);
        player = Player.Instance.transform;
    }

    private void Update() {
        if (player == null) player = Player.Instance.transform;

        innerBoundsRect = new Rect(innerBoundsOffset + transform.Position2D() - innerBounds / 2, innerBounds);

        if (!innerBoundsRect.Contains(player.transform.Position2D()))
        {
            transform.position = Vector2.Lerp(
                transform.Position2D(),
                player.transform.Position2D(),
                Time.deltaTime * followSpeed
            ).ToVec3(transform.position.z);
        }

        MaxCamera();
        
        var pos = transform.Position2D();
        if (pos.x > max.x)
            transform.SetPostion(x: max.x);
        else if (pos.x < min.x)
            transform.SetPostion(x: min.x);
        if (pos.y > max.y)
            transform.SetPostion(y: max.y);
        else if (pos.y < min.y)
            transform.SetPostion(y: min.y);
    }

    private void MaxCamera()
    {
        float distanceY = player.transform.position.y - transform.position.y;

        if (Mathf.Abs(distanceY) > 2.5f) {
            transform.position = new Vector3 (transform.position.x, player.transform.position.y - 2.5f*(distanceY/Mathf.Abs(distanceY)), transform.position.z);
        }
        
        float distanceX = player.transform.position.x - transform.position.x;

        if (Mathf.Abs(distanceX) > 5f) {
            transform.position = new Vector3 (player.transform.position.x - 5f*(distanceX/Mathf.Abs(distanceX)), transform.position.y , transform.position.z);
        }
        
        
        /*float distanceX = player.transform.position.x - transform.position.x;

        if (distanceX > 20.0f) {
            transform.position = new Vector3 (player.transform.position.x, player.transform.position.y, transform.position.z);
            return;
        }

        float newX = transform.position.x;
        if (distanceX <= 2 && distanceX >= -2) {}
        else if (distanceX <= 2.5f && distanceX >= -2.5f) {
            newX = transform.position.x + 0.5f * distanceX * Time.deltaTime;
        }else if (distanceX <= 3f && distanceX >= -3f) {
            newX = transform.position.x + 1.5f * distanceX * Time.deltaTime;
        } else {
            newX = transform.position.x + 2f * distanceX * Time.deltaTime;
        }*/
        
        /*transform.position = new Vector3 (transform.position.x, newY, transform.position.z);*/
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(innerBoundsOffset.ToVec3() + transform.Position2D().ToVec3(), new Vector3(innerBounds.x, innerBounds.y, 1));
    }
}
