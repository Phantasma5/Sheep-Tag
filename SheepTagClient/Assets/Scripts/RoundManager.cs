using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 0649
public class RoundManager : MonoBehaviour
{
    public const string playerDogPrefab = "Player_Dog";
    public const string playerSheepPrefab = "Player_Sheep";
    [SerializeField] private Text timerOutput;
    [SerializeField] private NetworkSync netSync;
    public bool IsReady
    {
        get;
        private set;
    } = false;
    public bool IsDog
    {
        get;
        private set;
    }

    private void Start()
    {
        SetPreferenceDog();
    }

    #region RPCs
    public void LobbyCountDownStart(float countDownLength)
    {
        if (!netSync.owned)
            return;

        Debug.Log("Lobby Countdown start: " + countDownLength);
        if(timerOutput)
        {
            if(timerRoutine != null)
            {
                StopCoroutine(timerRoutine);
            }
            timerRoutine = StartCoroutine(TimerOutputText(timerOutput, countDownLength, false));
        }
    }

    public void LobbyCountDownStop()
    {
        if (!netSync.owned)
            return;

        Debug.Log("Lobby Countdown stop");
        if(timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerOutput.gameObject.SetActive(false);
        }
    }

    public void RoundStart(float roundLength, bool itValue)
    {
        if (!netSync.owned)
            return;

        Debug.Log("Round start");

        if(References.client.readyScreen)
            References.client.readyScreen.SetActive(false);

        if(timerOutput)
        {
            if (timerRoutine != null)
            {
                StopCoroutine(timerRoutine);
            }
            timerRoutine = StartCoroutine(TimerOutputText(timerOutput, roundLength, false));
        }

        IsDog = itValue;

        if(IsDog)
        {
            References.client.myPlayer = SpawnPoint.SpawnDog();
        }
        else
        {
            References.client.myPlayer = SpawnPoint.SpawnSheep();
        }
        References.localPlayer = References.client.myPlayer;
        References.localPlayer.GetComponent<EnableOnLocalSpawn>()?.LocalSpawn();
        References.client.myPlayer.GetComponent<NetworkSync>().AddToArea(References.client.NetworkArea);
    }

    public void GameRoundOver()
    {
        if (!netSync.owned)
            return;

        //Countdown to disconnect, currently no countdown.
        Debug.Log("Round over");
    }

    public void LeaveGame()
    {
        if (!netSync.owned)
            return;

        Debug.Log("Leave");
        //Things I would do here are pretty much covered by TagClient.OnStatusDisconnected()
    }
    #endregion

    public void ReadyUp()
    {
        References.client.clientNet.CallRPC("ClientReady", UCNetwork.MessageReceiver.ServerOnly, -1, netSync.GetId());
        IsReady = true;
    }

    public void UnReady()
    {
        References.client.clientNet.CallRPC("ClientNotReady", UCNetwork.MessageReceiver.ServerOnly, -1, netSync.GetId());
        IsReady = false;
    }

    public void SetPreferenceDog()
    {
        References.client.clientNet.CallRPC("ItPreference", UCNetwork.MessageReceiver.ServerOnly, -1, new object[] { true, netSync.GetId() });
    }

    public void SetPreferenceSheep()
    {
        References.client.clientNet.CallRPC("ItPreference", UCNetwork.MessageReceiver.ServerOnly, -1, new object[] { false, netSync.GetId() });
    }

    private Coroutine timerRoutine;
    private IEnumerator TimerOutputText(Text output, float time, bool countUp = true)
    {
        output.gameObject.SetActive(true);

        for(float t = 0.0f; t < time; t += Time.deltaTime)
        {
            if(countUp)
            {
                output.text = Mathf.FloorToInt(t).ToString();
            }
            else
            {
                output.text = Mathf.CeilToInt(time - t).ToString();
            }
            yield return null;
        }
        
        if(countUp)
        {
            output.text = time.ToString();
        }
        else
        {
            output.text = "0";
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(netSync == null)
            netSync = GetComponent<NetworkSync>();
    }
#endif
}
