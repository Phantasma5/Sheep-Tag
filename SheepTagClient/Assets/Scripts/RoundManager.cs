using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 0649
public class RoundManager : MonoBehaviour
{
    [SerializeField] private string playerDogPrefab = "Player_Dog";
    [SerializeField] private string playerSheepPrefab = "Player_Sheep";
    [SerializeField] private Text timerOutput;
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
        Debug.Log("Lobby Countdown stop");
        if(timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerOutput.gameObject.SetActive(false);
        }
    }

    public void RoundStart(float roundLength, bool itValue)
    {
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
            References.client.myPlayer = References.client.clientNet.Instantiate(playerDogPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            References.client.myPlayer = References.client.clientNet.Instantiate(playerSheepPrefab, Vector3.zero, Quaternion.identity);
        }
        References.client.myPlayer.GetComponent<NetworkSync>().AddToArea(References.client.NetworkArea);
    }

    public void GameRoundOver()
    {
        //Countdown to disconnect, currently no countdown.
        Debug.Log("Round over");
    }

    public void LeaveGame()
    {
        Debug.Log("Leave");
        //Things I would do here are pretty much covered by TagClient.OnStatusDisconnected()
    }
    #endregion

    public void ReadyUp()
    {
        //Not yet implemented
        //Problem: How does the server RPC (or other method) know what client set it info? What is the proper way to do this?
        NetworkSync ns = GetComponent<NetworkSync>();
        if (ns)
        {
            References.client.clientNet.CallRPC("ClientReady", UCNetwork.MessageReceiver.ServerOnly, -1, ns.GetId());
            IsReady = true;
        }
    }

    public void UnReady()
    {
        NetworkSync ns = GetComponent<NetworkSync>();
        if (ns)
        {
            References.client.clientNet.CallRPC("ClientNotReady", UCNetwork.MessageReceiver.ServerOnly, -1, ns.GetId());
            IsReady = false;
        }
    }

    public void SetPreferenceDog()
    {
        NetworkSync ns = GetComponent<NetworkSync>();
        if (ns)
        {
            References.client.clientNet.CallRPC("ItPreference", UCNetwork.MessageReceiver.ServerOnly, -1, new object[] { true, ns.GetId() });
        }
    }

    public void SetPreferenceSheep()
    {
        NetworkSync ns = GetComponent<NetworkSync>();
        if (ns)
        {
            References.client.clientNet.CallRPC("ItPreference", UCNetwork.MessageReceiver.ServerOnly, -1, new object[] { false, ns.GetId() });
        }
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
}
