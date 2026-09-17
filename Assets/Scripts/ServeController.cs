using UnityEngine;
using UnityEngine.InputSystem;

public class ServeController : MonoBehaviour
{
    public enum ServeState { WaitingToServe, Tossing, SwingWindow, InPlay, Fault }

    public Transform server;
    public Transform racket;
    public Rigidbody ball;
    public Transform serveTarget;
    public Key tossKey = Key.Space;
    public Key swingKey = Key.Enter;

    public float tossSpeed = 5f;
    public float swingWindowDuration = 0.4f;
    public float serveFlightTime = 0.9f;
    public Vector3 handOffset = new Vector3(0.5f, 1.5f, 0.3f);
    public bool serverIsNearSide = true;

    public bool aiServer = false;
    public float aiTossDelay = 1f;
    public float aiSwingDelay = 0.15f;

    public ServeState State { get; private set; } = ServeState.WaitingToServe;

    int faultCount;
    float windowOpenTime;
    float stateEnterTime;
    RacketSwing racketSwing;

    void Awake()
    {
        RefreshRacket();
    }

    void RefreshRacket()
    {
        racketSwing = racket ? racket.GetComponent<RacketSwing>() : null;
    }

    void Update()
    {
        if (ball == null) return;
        var kb = Keyboard.current;

        switch (State)
        {
            case ServeState.WaitingToServe:
                HoldBallAtHand();
                if (aiServer)
                {
                    if (Time.time - stateEnterTime > aiTossDelay) Toss();
                }
                else if (kb != null && kb[tossKey].wasPressedThisFrame)
                {
                    Toss();
                }
                break;

            case ServeState.Tossing:
                if (ball.linearVelocity.y <= 0f)
                {
                    windowOpenTime = Time.time;
                    State = ServeState.SwingWindow;
                    Debug.Log("[Serve] Swing window open");
                }
                break;

            case ServeState.SwingWindow:
                if (aiServer)
                {
                    if (Time.time - windowOpenTime > aiSwingDelay) HitServe();
                }
                else if (kb != null && kb[swingKey].wasPressedThisFrame)
                {
                    HitServe();
                }
                else if (Time.time - windowOpenTime > swingWindowDuration)
                {
                    MissServe();
                }
                break;
        }
    }

    void HoldBallAtHand()
    {
        ball.isKinematic = true;
        Vector3 offset = new Vector3(handOffset.x, handOffset.y, serverIsNearSide ? handOffset.z : -handOffset.z);
        ball.position = server.position + offset;
    }

    void Toss()
    {
        ball.isKinematic = false;
        ball.linearVelocity = Vector3.up * tossSpeed;
        State = ServeState.Tossing;
        Debug.Log("[Serve] Toss");
    }

    void HitServe()
    {
        racketSwing?.PlaySwing();

        Vector3 start = ball.position;
        Vector3 target = serveTarget.position;
        float t = serveFlightTime;
        float g = Physics.gravity.y;

        Vector3 velocity = new Vector3(
            (target.x - start.x) / t,
            (target.y - start.y - 0.5f * g * t * t) / t,
            (target.z - start.z) / t
        );

        ball.linearVelocity = velocity;
        State = ServeState.InPlay;
        Debug.Log($"[Serve] Hit! velocity={velocity}");

        TennisScoreManager.Instance?.RecordHit(serverIsNearSide ? -1 : 1);
    }

    void MissServe()
    {
        faultCount++;
        State = ServeState.Fault;
        stateEnterTime = Time.time;
        Debug.Log(faultCount >= 2 ? "[Serve] Double fault!" : "[Serve] Fault");

        if (faultCount >= 2)
        {
            TennisScoreManager.Instance?.AwardPoint(serverIsNearSide ? 1 : -1);
        }

        Invoke(nameof(ResetForNextServe), 1.5f);
    }

    void ResetForNextServe()
    {
        if (faultCount >= 2) faultCount = 0;
        State = ServeState.WaitingToServe;
        stateEnterTime = Time.time;
    }

    // Called by TennisScoreManager at the start of every game to hand serve to the correct player.
    public void ConfigureForNewGame(Transform newServer, Transform newRacket, Transform newTarget, bool nearSide, bool ai)
    {
        server = newServer;
        racket = newRacket;
        serveTarget = newTarget;
        serverIsNearSide = nearSide;
        aiServer = ai;
        RefreshRacket();

        CancelInvoke(nameof(ResetForNextServe));
        faultCount = 0;
        State = ServeState.WaitingToServe;
        stateEnterTime = Time.time;
    }
}
