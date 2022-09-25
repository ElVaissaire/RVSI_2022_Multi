using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    [SerializeField] private int        m_vitesse = 5;
    [SerializeField] private Vector3    m_direction;
    [SerializeField] private Vector3    m_directionTmp;

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            m_directionTmp = GetDirection();
            if (m_directionTmp != Vector3.zero)
            {
                m_direction = m_directionTmp;
                if (IsClient && !IsServer)
                    EnvoiePositionRotationClientAuServeurServerRpc(Vector3.forward * m_vitesse * Time.deltaTime, m_direction);
                else
                {
                    transform.Translate(Vector3.forward * m_vitesse * Time.deltaTime); 
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_direction), 0.15f);
                }
            } 
        }
    }
    
    [ServerRpc]
    void EnvoiePositionRotationClientAuServeurServerRpc(Vector3 p_direction, Vector3 p_rotation)
    {
        transform.Translate(p_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(p_rotation), 0.15f);
    }
    Vector3 GetDirection()
    {
        Vector3 pos = Vector3.zero;
       
        if (Input.GetKey(KeyCode.UpArrow))
        {
            pos += Vector3.forward * m_vitesse * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            pos += Vector3.back * m_vitesse * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            pos += Vector3.left * m_vitesse * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            pos += Vector3.right * m_vitesse * Time.deltaTime;
        }
        return pos;
    }
}
