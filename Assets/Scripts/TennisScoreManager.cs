using UnityEngine;

public class TennisScoreManager : MonoBehaviour
{
    public static TennisScoreManager Instance { get; private set; }

    public BallController3D ball;

    [Header("서브 전환용 참조 (게임마다 서버가 바뀜)")]
    public ServeController serveController;
    public Transform player1;
    public Transform player1Racket;
    public Transform player2;
    public Transform player2Racket;
    public Transform serveTargetFromPlayer1;
    public Transform serveTargetFromPlayer2;

    public int setsToWinMatch = 2; // 3세트 중 2세트 선취 (best of 3)
    public int gamesToWinSet = 6;
    public int tiebreakTo = 7;

    public int Player1Points { get; private set; }
    public int Player2Points { get; private set; }
    public int Player1Games { get; private set; }
    public int Player2Games { get; private set; }
    public int Player1Sets { get; private set; }
    public int Player2Sets { get; private set; }

    public bool IsTiebreak { get; private set; }
    public bool MatchOver { get; private set; }

    int lastHitterSide;
    bool player1ServesThisGame = true;

    static readonly string[] PointNames = { "0", "15", "30" };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartGame();
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

    // side: -1 = Player1(가까운 쪽), +1 = Player2(먼 쪽)
    public void RecordHit(int side)
    {
        lastHitterSide = side;
    }

    void HandleBounced(int side, bool inBounds, int bounceCount)
    {
        if (MatchOver || lastHitterSide == 0) return;

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
        if (MatchOver || lastHitterSide == 0) return;
        AwardPoint(-lastHitterSide);
    }

    public void AwardPoint(int side)
    {
        if (MatchOver) return;
        lastHitterSide = 0;

        if (IsTiebreak)
        {
            AwardTiebreakPoint(side);
            return;
        }

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
            Debug.Log($"[점수] {(side < 0 ? "Player1" : "Player2")} 득점 -> {PointLabel()}");
            serveController?.ResetForNextPoint();
        }
    }

    void AwardTiebreakPoint(int side)
    {
        if (side < 0) Player1Points++;
        else Player2Points++;

        Debug.Log($"[점수] 타이브레이크 {(side < 0 ? "Player1" : "Player2")} 득점 -> {Player1Points}-{Player2Points}");

        bool p1Wins = Player1Points >= tiebreakTo && Player1Points - Player2Points >= 2;
        bool p2Wins = Player2Points >= tiebreakTo && Player2Points - Player1Points >= 2;

        if (p1Wins) WinGame(1, isTiebreakWin: true);
        else if (p2Wins) WinGame(2, isTiebreakWin: true);
        else serveController?.ResetForNextPoint();
    }

    void WinGame(int winner, bool isTiebreakWin = false)
    {
        if (winner == 1) Player1Games++; else Player2Games++;
        Debug.Log(isTiebreakWin
            ? $"[점수] Player{winner} 타이브레이크 승리! 게임 스코어: {Player1Games}-{Player2Games}"
            : $"[점수] Player{winner} 게임 승리! 게임 스코어: {Player1Games}-{Player2Games}");

        IsTiebreak = false;
        Player1Points = 0;
        Player2Points = 0;

        CheckSetWin();

        if (!MatchOver)
        {
            player1ServesThisGame = !player1ServesThisGame;
            StartGame();
        }
    }

    void CheckSetWin()
    {
        bool p1SetPoint = Player1Games >= gamesToWinSet && Player1Games - Player2Games >= 2;
        bool p2SetPoint = Player2Games >= gamesToWinSet && Player2Games - Player1Games >= 2;
        bool p1Wins7_5 = Player1Games == gamesToWinSet + 1 && Player2Games == gamesToWinSet - 1;
        bool p2Wins7_5 = Player2Games == gamesToWinSet + 1 && Player1Games == gamesToWinSet - 1;

        if (Player1Games == gamesToWinSet && Player2Games == gamesToWinSet)
        {
            IsTiebreak = true;
            Debug.Log("[점수] 6-6, 타이브레이크 시작!");
            return;
        }

        if (p1SetPoint || p1Wins7_5)
        {
            WinSet(1);
        }
        else if (p2SetPoint || p2Wins7_5)
        {
            WinSet(2);
        }
    }

    void WinSet(int winner)
    {
        if (winner == 1) Player1Sets++; else Player2Sets++;
        Debug.Log($"[점수] Player{winner} 세트 승리! 세트 스코어: {Player1Sets}-{Player2Sets}");

        Player1Games = 0;
        Player2Games = 0;

        if (Player1Sets >= setsToWinMatch || Player2Sets >= setsToWinMatch)
        {
            MatchOver = true;
            int matchWinner = Player1Sets > Player2Sets ? 1 : 2;
            Debug.Log($"[점수] 매치 종료! Player{matchWinner} 승리 (세트 {Player1Sets}-{Player2Sets})");
        }
    }

    void StartGame()
    {
        if (serveController == null) return;

        Transform server = player1ServesThisGame ? player1 : player2;
        Transform racket = player1ServesThisGame ? player1Racket : player2Racket;
        Transform target = player1ServesThisGame ? serveTargetFromPlayer1 : serveTargetFromPlayer2;

        serveController.ConfigureForNewGame(server, racket, target, nearSide: player1ServesThisGame, ai: !player1ServesThisGame);

        Debug.Log($"[점수] {(player1ServesThisGame ? "Player1" : "Player2")} 서브 게임 시작");
    }

    public string PointLabel()
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
