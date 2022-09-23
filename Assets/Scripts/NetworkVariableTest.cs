using Unity.Netcode;
using UnityEngine;

public class NetworkVariableTest : NetworkBehaviour
{
    private NetworkVariable<float> ServerNetworkVariable = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, // Everyone
        NetworkVariableWritePermission.Server);
    private NetworkVariable<float> ClientNetworkVariable = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, // Everyone
        NetworkVariableWritePermission.Owner);
    private float last_t = 0.0f;


    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            ServerNetworkVariable.Value = 0.0f;
            Debug.Log("Server's var initialized to: " + ServerNetworkVariable.Value);
        }
        else
        {
            ClientNetworkVariable.Value = 0.0f;
            Debug.Log("Client's var initialized to: " + ClientNetworkVariable.Value);
        }
    }

    void Update()
    {
        var t_now = Time.time;
        if (IsServer)
        {
            ServerNetworkVariable.Value = ServerNetworkVariable.Value + 0.1f;
            if (t_now - last_t > 0.5f)
            {
                last_t = t_now;
                Debug.Log("Server set its var to: " + ServerNetworkVariable.Value);
            }
        }
        else
        {
            ClientNetworkVariable.Value = ClientNetworkVariable.Value + 0.1f;
            if (t_now - last_t > 0.5f)
            {
                last_t = t_now;
                Debug.Log("Client set its var to: " + ClientNetworkVariable.Value);
            }
        }
    }
}
