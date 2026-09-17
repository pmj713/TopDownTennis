using UnityEngine;

public class TennisScoreManager : MonoBehaviour
{
    public static TennisScoreManager Instance { get; private set; }

    public BallController3D ball;

    public int Player1Points { get; private set; }
    public int Player2Points { get; private set; }
    public int Player1Games { get; private set; }
    public int Player2Games { get; private set; }

    int lastHitterSide;

    static readonly string[] PointNames = { "0", "15", "30" };

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        if (ball == null) return;
        ball.OnBounced += HandleBounced;
        ball.OnHitNet += HandleHitNet;
    }

    void OnDisable()
    {
        if (ball == null) return;
        ball.OnBounced -= HandleBounced;
        ball.OnHitNet -= HandleHitNet;
    }

    // side: -1 = Player1 (near), +1 = Player2 (far)
    public void RecordHit(int side)
    {
        lastHitterSide = side;
    }

    void HandleBounced(int side, bool inBounds, int bounceCount)
    {
        if (lastHitterSide == 0) return;

        if (!inBounds)
        {
            AwardPoint(-lastHitterSide);
        }
        else if (bounceCount >= 2)
        {
            AwardPoint(lastHitterSide);
        }
    }

    void HandleHitNet()
    {
        if (lastHitterSide == 0) return;
        AwardPoint(-lastHitterSide);
    }

    public void AwardPoint(int side)
    {
        lastHitterSide = 0;

        if (side < 0) Player1Points++;
        else Player2Points++;

        if (Player1Points >= 4 && Player1Points - Player2Points >= 2)
        {
            WinGame(1);
        }
        else if (Player2Points >= 4 && Player2Points - Player1Points >= 2)
        {
            WinGame(2);
        }
        else
        {
            Debug.Log($"[Score] Point to {(side < 0 ? "Player1" : "Player2")} -> {PointLabel()}");
        }
    }

    void WinGame(int winner)
    {
        if (winner == 1) Player1Games++; else Player2Games++;
        Debug.Log($"[Score] Game Player{winner}! Games: {Player1Games}-{Player2Games}");
        Player1Points = 0;
        Player2Points = 0;
    }

    string PointLabel()
    {
        int p1 = Player1Points;
        int p2 = Player2Points;

        if (p1 >= 3 && p2 >= 3)
        {
            int diff = p1 - p2;
            if (diff == 0) return "Deuce";
            return diff > 0 ? "Advantage Player1" : "Advantage Player2";
        }

        string l1 = p1 < 3 ? PointNames[p1] : "40";
        string l2 = p2 < 3 ? PointNames[p2] : "40";
        return $"{l1}-{l2}";
    }
}
