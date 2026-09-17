using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    public TennisScoreManager scoreManager;
    public TextMeshProUGUI scoreText;

    void Update()
    {
        if (scoreManager == null || scoreText == null) return;

        scoreText.text = scoreManager.MatchOver
            ? $"경기 종료 - 세트 {scoreManager.Player1Sets} : {scoreManager.Player2Sets}"
            : $"{scoreManager.PointLabel()}\n게임 {scoreManager.Player1Games} : {scoreManager.Player2Games}   세트 {scoreManager.Player1Sets} : {scoreManager.Player2Sets}";
    }
}
