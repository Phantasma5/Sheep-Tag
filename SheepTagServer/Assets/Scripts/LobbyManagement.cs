using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManagement : MonoBehaviour
{
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

    #endregion
    #region Game Ending Variables

    #endregion


    private void FixedUpdate()
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
        if(playersReady == server.players.Count && playersReady > minPlayerCount && countdownRoutine == null)
        {
            countdownRoutine = StartCoroutine(Countdown(countDownLength, () =>
            {
                server.serverNet.CallRPC("RoundStart", UCNetwork.MessageReceiver.AllClients, -1);
            }));
            server.serverNet.CallRPC("LobbyCountdownStart", UCNetwork.MessageReceiver.AllClients, -1, countDownLength);
        }
        else if(countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
            server.serverNet.CallRPC("LobbyCountdownStop", UCNetwork.MessageReceiver.AllClients, -1);
        }
    }

    private void OnGameRunning()
    {

    }

    private void GameLogic_Running()
    {
        
    }

    private void OnGameOver()
    {

    }

    private void GameLogic_GameOver()
    {

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
