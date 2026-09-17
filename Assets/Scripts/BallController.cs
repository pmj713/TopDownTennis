using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 6f;
    public float speedIncreasePerHit = 0.4f;
    public float maxSpeed = 16f;
    public float boundX = 8.5f;
    public float relaunchDelay = 1f;

    Rigidbody2D rb;
    Vector3 startPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    void Start()
    {
        Launch();
    }

    void Launch()
    {
        float dirX = Random.value < 0.5f ? -1f : 1f;
        float dirY = Random.Range(-0.5f, 0.5f);
        Vector2 dir = new Vector2(dirX, dirY).normalized;
        rb.linearVelocity = dir * initialSpeed;
    }

    void FixedUpdate()
    {
        if (transform.position.x < -boundX)
        {
            GameManager.Instance.AddPoint(toRightPlayer: true);
            ResetBall();
        }
        else if (transform.position.x > boundX)
        {
            GameManager.Instance.AddPoint(toRightPlayer: false);
            ResetBall();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Paddle")) return;

        Vector2 v = rb.linearVelocity;
        float newSpeed = Mathf.Min(v.magnitude + speedIncreasePerHit, maxSpeed);
        rb.linearVelocity = v.normalized * newSpeed;
    }

    void ResetBall()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = startPosition;
        Invoke(nameof(Launch), relaunchDelay);
    }
}
