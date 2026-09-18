using TMPro;
using UnityEngine;

// 멀티플레이 오버레이 패널: 방 만들기 / 코드로 참가하기 버튼과 상태 텍스트를 관리한다.
// 실제 세션 생성/참가 로직은 MultiplayerBootstrap에 위임한다.
public class MultiplayerPanelController : MonoBehaviour
{
    public MainMenuController mainMenuController;
    public TextMeshProUGUI statusText;
    public TMP_InputField codeInputField;

    void OnEnable()
    {
        statusText.text = string.Empty;
        codeInputField.text = string.Empty;

        if (MultiplayerBootstrap.Instance != null)
        {
            MultiplayerBootstrap.Instance.OnStatusChanged += HandleStatusChanged;
            MultiplayerBootstrap.Instance.OnJoinFailed += HandleJoinFailed;
        }
    }

    void OnDisable()
    {
        if (MultiplayerBootstrap.Instance != null)
        {
            MultiplayerBootstrap.Instance.OnStatusChanged -= HandleStatusChanged;
            MultiplayerBootstrap.Instance.OnJoinFailed -= HandleJoinFailed;
        }
    }

    public async void OnHostClicked()
    {
        await MultiplayerBootstrap.Instance.HostSessionAsync();
    }

    public async void OnJoinClicked()
    {
        await MultiplayerBootstrap.Instance.JoinSessionAsync(codeInputField.text.Trim());
    }

    public void OnBackClicked()
    {
        MultiplayerBootstrap.Instance.LeaveSession();
        mainMenuController.HideMultiplayerPanel();
    }

    void HandleStatusChanged(string message)
    {
        statusText.text = message;
    }

    void HandleJoinFailed(string message)
    {
        statusText.text = message;
    }
}
