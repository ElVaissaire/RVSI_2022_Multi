using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{
    public static PlayerBehavior player_S;

    [SerializeField] private int        m_vitesse = 5;
    [SerializeField] private Vector3    m_direction;
    [SerializeField] private Vector3    m_directionTmp;

    [SerializeField] public BouleNeige  m_bouleNeige;
    [SerializeField] private bool       m_PouvoirTirer = true;

    [SerializeField] public int         m_score = 100;

    private void Awake()
    {
        player_S = this;
    }

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
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_direction), 0.3f);
                }
            }

            LancerBouleNeige();
        }
    }
    
    [ServerRpc]
    void EnvoiePositionRotationClientAuServeurServerRpc(Vector3 p_direction, Vector3 p_rotation)
    {
        transform.Translate(p_direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(p_rotation), 0.3f);
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

    void LancerBouleNeige()
    {
        if (Input.GetKey(KeyCode.Space) && m_PouvoirTirer && m_direction != Vector3.zero)
        {
            m_PouvoirTirer = false;
            Vector3 position = new Vector3(transform.position.x + (m_direction.x * 30f), 0, transform.position.z + (m_direction.z * 30f));

            InstancierBouleNeigeServerRpc(position, m_direction, m_vitesse*100);    // On laisse le serveur instancier la boule de neige
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_PouvoirTirer = true;
        }
    }

    [ServerRpc]
    void InstancierBouleNeigeServerRpc(Vector3 p_position, Vector3 p_direction, float p_vitesse)
    {
        m_bouleNeige.transform.position = p_position;
        m_bouleNeige.m_vitesseBouleNeige = p_vitesse;
        m_bouleNeige.m_direction = p_direction;
        
        BouleNeige bouleNeige = Instantiate(m_bouleNeige);
        bouleNeige.GetComponent<NetworkObject>().Spawn();   // Mettre l'instance de boule de neige sur le serveur pour que le client puisse la voir
    }
}
