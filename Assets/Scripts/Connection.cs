using Mirror;
using UnityEngine;


public class Connection : MonoBehaviour
{
    public NetworkManager NetworkManager;

    private void Start()
    {
        if (!Application.isBatchMode)
        {
            NetworkManager.StartClient();
        }
    }


    public void JoinClient()
    {

        NetworkManager.networkAddress = "localhost";
        NetworkManager.StartClient();
    }
}
