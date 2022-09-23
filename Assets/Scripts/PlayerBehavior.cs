using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    [SerializeField] private int        m_speed;
    [SerializeField] private NetworkVariable<Vector3>    m_position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            Destroy(this);
    }
    */

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
                PlayerControl();
        }
        

        /*
        if (IsOwner)
        {
            PlayerControl();

            if (NetworkManager.Singleton.IsServer)
            {
                m_position.Value = NetworkManager.Singleton.LocalClient.PlayerObject.transform.position;
                SetPositionServerRpc();
            }
            else
                SetPositionClientRpc();
        }
        else
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = m_position.Value;//*/
    }


    /*[ServerRpc]
    void SetPositionServerRpc(ServerRpcParams rpcParams = default)
    {
        m_position.Value = NetworkManager.Singleton.LocalClient.PlayerObject.transform.position;
    }

    [ClientRpc]
    void SetPositionClientRpc(ClientRpcParams rpcParams = default)
    {
        m_position.Value = NetworkManager.Singleton.LocalClient.PlayerObject.transform.position;
    }*/


    void PlayerControl()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * m_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * m_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * m_speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * m_speed * Time.deltaTime);
        }
    }
}
