using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TagClient : MonoBehaviour
{
    public ClientNetwork clientNet;
    static TagClient instance = null;
    private bool loginInProcess = false;
    public GameObject loginScreen;
    public GameObject myPlayer;
    public bool it;

    // Singleton support
    public static TagClient GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("ExampleClient is uninitialized");
            return null;
        }
        return instance;
    }
    void Awake()
    {
        // Make sure we have a ClientNetwork to use
        if (clientNet == null)
        {
            clientNet = GetComponent<ClientNetwork>();
        }
        if (clientNet == null)
        {
            clientNet = (ClientNetwork)gameObject.AddComponent(typeof(ClientNetwork));
        }
    }
    public void ConnectToServer(string aServerAddress, int aPort)
    {
        if (loginInProcess)
        {
            return;
        }
        loginInProcess = true;

        ClientNetwork.port = aPort;
        clientNet.Connect(aServerAddress, ClientNetwork.port, "", "", "", 0);
    }
    public void UpdateState(int x, int y, string player)
    {
        // Update the visuals for the game
    }
    public void NewClientConnected(long aClientId, string aValue)
    {
        Debug.Log("RPC NewClientConnected has been called with " + aClientId + " " + aValue);
    }
    void OnNetStatusNone()
    {
        Debug.Log("OnNetStatusNone called");
    }
    void OnNetStatusInitiatedConnect()
    {
        Debug.Log("OnNetStatusInitiatedConnect called");
    }
    void OnNetStatusReceivedInitiation()
    {
        Debug.Log("OnNetStatusReceivedInitiation called");
    }
    void OnNetStatusRespondedAwaitingApproval()
    {
        Debug.Log("OnNetStatusRespondedAwaitingApproval called");
    }
    void OnNetStatusRespondedConnect()
    {
        Debug.Log("OnNetStatusRespondedConnect called");
    }
    void OnNetStatusConnected()
    {
        loginScreen.SetActive(false);
        Debug.Log("OnNetStatusConnected called");

        clientNet.AddToArea(1);
    }
    void OnNetStatusDisconnecting()
    {
        Debug.Log("OnNetStatusDisconnecting called");

        if (myPlayer)
        {
            clientNet.Destroy(myPlayer.GetComponent<NetworkSync>().GetId());
        }
    }
    void OnNetStatusDisconnected()
    {
        Debug.Log("OnNetStatusDisconnected called");
        SceneManager.LoadScene("Client");

        loginInProcess = false;

        if (myPlayer)
        {
            clientNet.Destroy(myPlayer.GetComponent<NetworkSync>().GetId());
        }
    }
    public void OnChangeArea()
    {
        Debug.Log("OnChangeArea called");

        // Tell the server we are ready
        myPlayer = clientNet.Instantiate("Player", Vector3.zero, Quaternion.identity);
        myPlayer.GetComponent<NetworkSync>().AddToArea(1);
    }
    public void AreaInitialized()
    {
        Debug.Log("AreaInitialized called");
    }
    void OnDestroy()
    {
        if (myPlayer)
        {
            clientNet.Destroy(myPlayer.GetComponent<NetworkSync>().GetId());
        }
        if (clientNet.IsConnected())
        {
            clientNet.Disconnect("Peace out");
        }
    }
}//end class


