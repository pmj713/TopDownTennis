using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameplaySceneName = "SampleScene";

    public void OnSinglePlayClicked()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnTutorialClicked()
    {
        Debug.Log("[Menu] 튜토리얼 - 준비 중입니다.");
    }

    public void OnMultiplayerClicked()
    {
        Debug.Log("[Menu] 멀티플레이 - 준비 중입니다.");
    }
}
