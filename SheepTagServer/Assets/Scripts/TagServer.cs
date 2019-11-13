using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class TagServer : MonoBehaviour
{
    public static TagServer instance;

    public ServerNetwork serverNet;

    private ServerNetwork serverNetwork;
    public int portNumber = 603;

    // Stores a player
    [System.Serializable]
    public class Player
    {
        public long clientId;
        public string playerName;
        public bool isReady;
        public bool isConnected;
        public bool wantDog;
    }
    /*[HideInInspector]*/ public List<Player> players = new List<Player>();
    int currentActivePlayer;
    private void Start()
    {
        serverNetwork = GetComponent<ServerNetwork>();
    }
    void Awake()
    {
        instance = this;

        // Initialization of the server network
        ServerNetwork.port = portNumber;
        if (serverNet == null)
        {
            serverNet = GetComponent<ServerNetwork>();
        }
        if (serverNet == null)
        {
            serverNet = (ServerNetwork)gameObject.AddComponent(typeof(ServerNetwork));
            Debug.Log("ServerNetwork component added.");
        }

        //serverNet.EnableLogging("rpcLog.txt");
    }
    void ConnectionRequest(ServerNetwork.ConnectionRequestInfo data)
    {
        Debug.Log("Connection request from " + data.username);

        // We either need to approve a connection or deny it
        //if (players.Count < 2)
        {
            Player newPlayer = new Player();
            newPlayer.clientId = data.id;
            newPlayer.playerName = data.username;
            newPlayer.isConnected = false;
            newPlayer.isReady = false;
            players.Add(newPlayer);

            serverNet.ConnectionApproved(data.id);
        }
        /*
        else
        {
            serverNet.ConnectionDenied(data.id);
        }
        */
    }
    void OnClientConnected(long aClientId)
    {
        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == aClientId)
            {
                p.isConnected = true;
            }
        }
        /*
        serverNet.CallRPC("RPCTest", UCNetwork.MessageReceiver.AllClients, -1, 45);
        ServerNetwork.ClientData data = serverNet.GetClientData(serverNet.SendingClientId);
        serverNet.CallRPC("NewClientConnected", UCNetwork.MessageReceiver.AllClients, -1, aClientId, "bob");
        */
    }
    public void OnInstantiateNetworkObject(ServerNetwork.IntantiateObjectData aData)
    {
        //whenever an object was instatiated this gets called
    }
    void OnClientDisconnected(long aClientId)
    {
        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == aClientId)
            {
                p.isConnected = false;
                p.isReady = false;
                GetComponent<TagDetection>().CheckIt();
            }
        }
    }
}//end class
