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
        // min = new Vector2(-100, -100);
        // max = new Vector2(100, 100);
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

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(innerBoundsOffset.ToVec3() + transform.Position2D().ToVec3(), new Vector3(innerBounds.x, innerBounds.y, 1));
    }
}
