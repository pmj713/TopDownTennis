using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
    public Rigidbody ball;
    public float moveSpeed = 4f;
    public float halfCourtWidth = 4.115f;
    public float halfCourtLength = 11.885f;
    public bool nearSide = false;

    void Update()
    {
        if (ball == null) return;

        Vector3 pos = transform.position;

        float targetX = Mathf.Clamp(ball.position.x, -halfCourtWidth + 0.4f, halfCourtWidth - 0.4f);
        pos.x = Mathf.MoveTowards(pos.x, targetX, moveSpeed * Time.deltaTime);

        float homeZ = nearSide ? -halfCourtLength * 0.5f : halfCourtLength * 0.5f;
        pos.z = Mathf.MoveTowards(pos.z, homeZ, moveSpeed * 0.5f * Time.deltaTime);

        transform.position = pos;
    }
}
