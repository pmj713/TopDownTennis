using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

// 멀티플레이 세션(초대 코드) 연결을 담당하는 싱글턴. 씬이 바뀌어도 살아있어야 하므로
// DontDestroyOnLoad로 유지하고, 두 번째 플레이어가 접속하면 온라인 게임 씬으로 전환한다.
public class MultiplayerBootstrap : MonoBehaviour
{
    public static MultiplayerBootstrap Instance { get; private set; }

    public string onlineGameSceneName = "OnlineGameScene";

    public event Action<string> OnStatusChanged;
    public event Action<string> OnJoinFailed;

    IHostSession hostSession;
    ISession currentSession;
    bool servicesReady;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async Task EnsureServicesReadyAsync()
    {
        if (servicesReady) return;

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        servicesReady = true;
    }

    public async Task<string> HostSessionAsync()
    {
        await EnsureServicesReadyAsync();

        OnStatusChanged?.Invoke("방을 만드는 중...");

        try
        {
            var options = new SessionOptions { MaxPlayers = 2 }.WithRelayNetwork();
            hostSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            currentSession = hostSession;

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;

            OnStatusChanged?.Invoke($"코드: {hostSession.Code}  상대를 기다리는 중...");
            return hostSession.Code;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnStatusChanged?.Invoke("방 만들기에 실패했습니다: " + e.Message);
            return null;
        }
    }

    public async Task JoinSessionAsync(string code)
    {
        await EnsureServicesReadyAsync();

        OnStatusChanged?.Invoke("참가하는 중...");

        try
        {
            currentSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);
            OnStatusChanged?.Invoke("연결됨! 상대를 기다리는 중...");
        }
        catch (SessionException e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke(FriendlyError(e));
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            OnJoinFailed?.Invoke("참가에 실패했습니다: " + e.Message);
        }
    }

    void HandleClientConnected(ulong clientId)
    {
        // 호스트 자신 접속 콜백(1명)과 상대 접속 콜백(2명)이 모두 이 이벤트를 통해 들어온다.
        // 정확히 2명이 모이면 온라인 게임 씬으로 전환(호스트가 로드하면 Netcode가 클라이언트도 같이 전환시킴).
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.SceneManager.LoadScene(onlineGameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    string FriendlyError(SessionException e)
    {
        return e.Error switch
        {
            SessionError.SessionNotFound => "코드가 맞는지 확인해주세요.",
            SessionError.SessionDeleted => "방이 이미 종료되었습니다.",
            _ => "연결에 실패했습니다. 다시 시도해주세요.",
        };
    }

    public async void LeaveSession()
    {
        if (currentSession == null) return;
        try { await currentSession.LeaveAsync(); } catch { /* 이미 끊어졌으면 무시 */ }
        currentSession = null;
        hostSession = null;
    }
}
