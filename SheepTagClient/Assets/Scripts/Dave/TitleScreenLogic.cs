using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TitleScreenLogic : MonoBehaviour {

    public Text userName;
    public Text server;
    public Text port;
    public TagClient client;

    public UnityEvent OnInvalidConnect;

    public void Connect()
    {
        client.ConnectToServer(server.text, int.Parse(port.text));
    }

    public void ConnectUsername()
    {
        string user = userName.text.Trim();
        string address = server.text;
        int portNum;

        if (!int.TryParse(port.text, out portNum) || user == string.Empty || address == string.Empty)
        {
            OnInvalidConnect.Invoke();
        }
        else
        {
            client.ConnectToServer(user, address, portNum);
        }
    }
}
