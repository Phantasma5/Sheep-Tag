using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private string playerPrefab = "Player";
    [SerializeField] private Text timerOutput;

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
        }
    }

    public void RoundStart(float roundLength)
    {
        Debug.Log("Round start");
        if(timerOutput)
        {
            if (timerRoutine != null)
            {
                StopCoroutine(timerRoutine);
            }
            timerRoutine = StartCoroutine(TimerOutputText(timerOutput, roundLength, false));
        }

        References.client.myPlayer = References.client.clientNet.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
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
