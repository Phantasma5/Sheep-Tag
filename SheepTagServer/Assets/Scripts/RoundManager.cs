using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private string roundManagerObjectName = "RoundManager";
    [SerializeField] private string playerDogPrefab = "Player_Dog";
    [SerializeField] private string playerSheepPrefab = "Player_Sheep";
    [SerializeField] private float percentageDogs = 0.25f;

    [SerializeField] TagServer server;
    [SerializeField] private TagDetection tagDetect;
    [SerializeField] private int minPlayerCount = 2;

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
        int playerCount = 0;
        foreach(var netObj in server.serverNet.networkObjects)
        {
            if(netObj.Value.prefabName == roundManagerObjectName)
            {
                playerCount++;
                if (netObj.Value.ready)
                    playersReady++;
            }
        }
        if(playersReady == playerCount && playerCount >= minPlayerCount)
        {
            if (countdownRoutine == null)
            {
                countdownRoutine = StartCoroutine(Countdown(countDownLength, () => CurrentPhase = GamePhase.RUNNING ));

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
        //Close server off to new players

        List<int> dogPlayers = new List<int>();
        List<int> sheepPlayers = new List<int>();

        foreach (var netObject in server.serverNet.networkObjects)
        {
            if(netObject.Value.prefabName == roundManagerObjectName)
            {
                if(netObject.Value.it)
                {
                    dogPlayers.Add(netObject.Key);
                }
                else
                {
                    sheepPlayers.Add(netObject.Key);
                }
            }
        }

        int dogCount = Mathf.Max(1, Mathf.FloorToInt(percentageDogs * (dogPlayers.Count + sheepPlayers.Count)));
        Debug.Log("Selecting " + dogCount + " dogs from " + dogPlayers.Count + " willing dogs and " + sheepPlayers.Count + " willing sheep.");
        while(dogCount > 0)
        {
            int newDog;
            if (dogPlayers.Count > 0)
            {
                int index = Random.Range(0, dogPlayers.Count);
                newDog = dogPlayers[index];
                dogPlayers.RemoveAt(index);
            }
            else
            {
                int index = Random.Range(0, sheepPlayers.Count);
                newDog = sheepPlayers[index];
                sheepPlayers.RemoveAt(index);
            }
            server.serverNet.networkObjects[newDog].it = true;
            server.serverNet.CallRPC("RoundStart", UCNetwork.MessageReceiver.AllClients, newDog, new object[] { gameLength, true });
            --dogCount;
        }

        sheepPlayers.AddRange(dogPlayers);
        Debug.Log("Remaining sheep players: " + sheepPlayers.Count);
        while(sheepPlayers.Count > 0)
        {
            server.serverNet.networkObjects[sheepPlayers[0]].it = false;
            server.serverNet.CallRPC("RoundStart", UCNetwork.MessageReceiver.AllClients, sheepPlayers[0], new object[] { gameLength, false });
            sheepPlayers.RemoveAt(0);
        }

        tagDetect.enabled = true;
        gameStartTime = Time.timeSinceLevelLoad;
    }

    private void GameLogic_Running()
    {
        bool moveToGameOver = true;
        bool atLeastOneSheep = false;
        bool allCaptured = true;
        bool allSafe = true;
        foreach(var netObj in server.serverNet.networkObjects)
        {
            Debug.Log(netObj.Value.prefabName + " | " + netObj.Value.condition);

            if(netObj.Value.prefabName == roundManagerObjectName)
            {
                moveToGameOver = false;
            }
            else if(netObj.Value.prefabName == playerSheepPrefab)
            {
                atLeastOneSheep = true;
                if (netObj.Value.condition != "CAPTURED")
                {
                    allCaptured = false;
                }
                if (netObj.Value.condition != "SAFE")
                {
                    allSafe = false;
                }
            }
        }

        if((allCaptured || allSafe) && atLeastOneSheep)
        {
            //Send what kind of game over
            Debug.Log("All captured" + allCaptured + "\nAll safe: " + allSafe);
            moveToGameOver = true;
        }

        if(Time.timeSinceLevelLoad - gameStartTime >= gameLength || moveToGameOver)
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
    }
    
    public void ClientReady(int clientId)
    {
        if (CurrentPhase != GamePhase.LOBBY)
            return;

        Debug.Log("Client ready - " + clientId);
        if(server.serverNet.networkObjects.ContainsKey(clientId))
        {
            server.serverNet.networkObjects[clientId].ready = true;
        }
    }

    public void ClientNotReady(int clientId)
    {
        if (CurrentPhase != GamePhase.LOBBY)
            return;

        Debug.Log("Client not ready - " + clientId);
        if (server.serverNet.networkObjects.ContainsKey(clientId))
        {
            server.serverNet.networkObjects[clientId].ready = false;
        }
    }

    public void ItPreference(bool value, int clientId)
    {
        if (CurrentPhase != GamePhase.LOBBY)
            return;

        if (server.serverNet.networkObjects.ContainsKey(clientId))
        {
            server.serverNet.networkObjects[clientId].it = value;
        }
    }

    private IEnumerator Countdown(float t, VoidDelegate OnCountDownComplete)
    {
        yield return new WaitForSeconds(t);
        OnCountDownComplete.Invoke();
        countdownRoutine = null;
    }

    public void ForceGameStart()
    {
        if(CurrentPhase == GamePhase.LOBBY)
        {
            foreach (var netObject in server.serverNet.networkObjects)
            {
                if (netObject.Value.prefabName == roundManagerObjectName)
                {
                    server.serverNet.CallRPC("RoundStart", UCNetwork.MessageReceiver.AllClients, netObject.Value.networkId, gameLength);
                }
            }
            CurrentPhase = GamePhase.RUNNING;
        }
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
