using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private string roundManagerObjectName = "RoundManager";

    [SerializeField] TagServer server;
    [SerializeField] private TagDetection tagDetect;
    [SerializeField] private int minPlayerCount = 1;

    public enum GamePhase
    {
        LOBBY,
        RUNNING,
        GAMEOVER
    }
    [SerializeField] private GamePhase _currentPhase;
    public GamePhase CurrentPhase
    {
        get { return _currentPhase; }
        set
        {
            if(_currentPhase != value)
            {
                _currentPhase = value;
                if(_currentPhase == GamePhase.RUNNING)
                {
                    OnGameRunning();
                }
                else if(_currentPhase == GamePhase.GAMEOVER)
                {
                    OnGameOver();
                }
            }
        }
    }

    #region Lobby Variables
    [SerializeField] private float countDownLength = 5.0f;
    private Coroutine countdownRoutine;
    private delegate void VoidDelegate();
    #endregion
    #region Game Variables
    [SerializeField] private float gameLength = 36000.0f;
    private float gameStartTime;
    #endregion
    #region Game Ending Variables

    #endregion

    private void Awake()
    {
        tagDetect.enabled = false;
    }

    private void Update()
    {
        //State machine, basically.
        switch(_currentPhase)
        {
            case GamePhase.LOBBY:
                GameLogic_Lobby();
                break;
            case GamePhase.RUNNING:
                GameLogic_Running();
                break;
            case GamePhase.GAMEOVER:
                GameLogic_GameOver();
                break;
        }
    }

    private void GameLogic_Lobby()
    {
        int playersReady = 0;
        foreach(TagServer.Player player in server.players)
        {
            if (player.isReady)
                playersReady++;
        }
        if(playersReady == server.players.Count && playersReady >= minPlayerCount)
        {
            if (countdownRoutine == null)
            {
                countdownRoutine = StartCoroutine(Countdown(countDownLength, () =>
                {
                //This lambda function is called when the countdown coroutine finishes.

                foreach (var netObject in server.serverNet.networkObjects)
                    {
                        if (netObject.Value.prefabName == roundManagerObjectName)
                        {
                            server.serverNet.CallRPC("RoundStart", UCNetwork.MessageReceiver.AllClients, netObject.Value.networkId, gameLength);
                        }
                    }
                    CurrentPhase = GamePhase.RUNNING;
                //End lambda callback function.
            }));

                foreach (var netObject in server.serverNet.networkObjects)
                {
                    if (netObject.Value.prefabName == roundManagerObjectName)
                    {
                        server.serverNet.CallRPC("LobbyCountDownStart", UCNetwork.MessageReceiver.AllClients, netObject.Value.networkId, countDownLength);
                    }
                }
            }
        }
        else if(countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
            foreach (var netObject in server.serverNet.networkObjects)
            {
                if (netObject.Value.prefabName == roundManagerObjectName)
                {
                    tagDetect.enabled = false;
                    server.serverNet.CallRPC("LobbyCountDownStop", UCNetwork.MessageReceiver.AllClients, netObject.Value.networkId);
                }
            }
        }
    }

    private void OnGameRunning()
    {
        tagDetect.enabled = true;
        gameStartTime = Time.timeSinceLevelLoad;
    }

    private void GameLogic_Running()
    {
        if(Time.timeSinceLevelLoad - gameStartTime >= gameLength)
        {
            CurrentPhase = GamePhase.GAMEOVER;
        }
    }

    private void OnGameOver()
    {
        foreach (var netObject in server.serverNet.networkObjects)
        {
            if (netObject.Value.prefabName == roundManagerObjectName)
            {
                server.serverNet.CallRPC("GameRoundOver", UCNetwork.MessageReceiver.AllClients, netObject.Value.networkId);
            }
        }
    }

    private void GameLogic_GameOver()
    {
        foreach (var netObject in server.serverNet.networkObjects)
        {
            if (netObject.Value.prefabName == roundManagerObjectName)
            {
                server.serverNet.CallRPC("LeaveGame", UCNetwork.MessageReceiver.AllClients, netObject.Value.networkId);
            }
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        //Restart Server/Scene/whatever
    }

    private IEnumerator Countdown(float t, VoidDelegate OnCountDownComplete)
    {
        yield return new WaitForSeconds(t);
        OnCountDownComplete.Invoke();
        countdownRoutine = null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(tagDetect == null)
        {
            tagDetect = GetComponent<TagDetection>();
        }
        if(server == null)
        {
            server = GetComponent<TagServer>();
        }
    }
#endif
}
