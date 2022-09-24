using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    [SerializeField] private int        m_speed = 5;
    [SerializeField] private Vector3    m_deplacement;

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            m_deplacement = GetDeplacement();
            if (IsClient && !IsServer)
            {
                EnvoiePositionClientAuServeurServerRpc(m_deplacement);
            }
            else
            {
                transform.Translate(m_deplacement);
            }
        }
    }
    [ServerRpc]
    void EnvoiePositionClientAuServeurServerRpc(Vector3 p_position)
    {
        transform.Translate(p_position);
    }
    Vector3 GetDeplacement()
    {
        Vector3 pos = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pos += Vector3.forward * m_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pos += Vector3.back * m_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos += Vector3.left * m_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos += Vector3.right * m_speed * Time.deltaTime;
        }
        return pos;
    }
}
