using UnityEngine;
using UnityEngine.InputSystem;

public class RallyHitController : MonoBehaviour
{
    public Transform racket;
    public Rigidbody ball;
    public BallController3D ballController;
    public ServeController serveController;
    public bool nearSide = true;

    public Key swingKey = Key.Enter;
    public Key aimLeftKey = Key.A;
    public Key aimRightKey = Key.D;

    public float hitRange = 1.5f;
    public float returnFlightTime = 0.8f;
    public float aimSpread = 2.5f;
    public float targetDepth = 6f;

    public bool aiControlled = false;
    public float aiAimJitter = 0.6f;

    RacketSwing racketSwing;

    void Awake()
    {
        racketSwing = racket ? racket.GetComponent<RacketSwing>() : null;
    }

    void Update()
    {
        if (ball == null || ballController == null) return;

        // 지금 내가 서브하는 중이면(토스/스윙 대기 등) 랠리 리턴으로 끼어들지 않음
        bool iAmCurrentlyServing = serveController != null
            && serveController.server == transform
            && serveController.State != ServeController.ServeState.InPlay;
        if (iAmCurrentlyServing) return;

        bool ballOnMySide = nearSide ? ballController.CurrentSide < 0 : ballController.CurrentSide > 0;
        if (!ballOnMySide) return;

        float dist = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(ball.position.x, ball.position.z));
        if (dist > hitRange) return;

        if (aiControlled)
        {
            PerformHit(Random.Range(-aiAimJitter, aiAimJitter));
            return;
        }

        var kb = Keyboard.current;
        if (kb == null || !kb[swingKey].wasPressedThisFrame) return;

        float aim = 0f;
        if (kb[aimLeftKey].isPressed) aim -= 1f;
        if (kb[aimRightKey].isPressed) aim += 1f;

        PerformHit(aim);
    }

    void PerformHit(float aim)
    {
        float targetZ = nearSide ? targetDepth : -targetDepth;
        Vector3 target = new Vector3(Mathf.Clamp(aim * aimSpread, -3.8f, 3.8f), 0.1f, targetZ);

        // 공이 내 오른쪽에 있으면 포핸드(오른쪽->왼쪽), 왼쪽에 있으면 백핸드(왼쪽->오른쪽)
        bool ballOnMyRight = nearSide
            ? ball.position.x > transform.position.x
            : ball.position.x < transform.position.x;
        racketSwing?.PlaySwing(mirrored: !ballOnMyRight);

        Vector3 start = ball.position;
        float t = returnFlightTime;
        float g = Physics.gravity.y;

        Vector3 velocity = new Vector3(
            (target.x - start.x) / t,
            (target.y - start.y - 0.5f * g * t * t) / t,
            (target.z - start.z) / t
        );

        ball.linearVelocity = velocity;
        Debug.Log($"[Rally] {name} returned the ball -> target={target}, velocity={velocity}");

        TennisScoreManager.Instance?.RecordHit(nearSide ? -1 : 1);
    }
}
