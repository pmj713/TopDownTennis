using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int leftScore { get; private set; }
    public int rightScore { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void AddPoint(bool toRightPlayer)
    {
        if (toRightPlayer) rightScore++;
        else leftScore++;

        Debug.Log($"Score - Left: {leftScore}  Right: {rightScore}");
    }
}
