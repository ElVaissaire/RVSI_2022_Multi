using Unity.Netcode;
using UnityEngine;

public class PlayerBehavior : NetworkBehaviour
{

    [SerializeField] private int        m_vitesse = 5;
    [SerializeField] private Vector3    m_direction;
    [SerializeField] private Vector3    m_directionTmp;

    [SerializeField] private GameObject m_snowballPrefab;
    [SerializeField] private Transform  m_snowballSpawn;

    [SerializeField] public int         m_vie = 100;

    private NetworkVariable<int>        m_networkID = new NetworkVariable<int>();
    public int                          m_ID = -1;

    private void Awake()
    {
        m_ID = -1;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            m_ID = GameManager.Instance.AddPlayer(gameObject);
            m_networkID.Value = m_ID;
        }
        else
        {
            m_ID = m_networkID.Value;
            GetComponent<MeshRenderer>().material.color = GameManager.Instance.GetColorByID(m_ID);
        }
    }

    public override void OnNetworkDespawn()
    {
        if(IsOwner)
            GameManager.Instance.DeletePlayer(m_ID);
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
        if (Input.GetKeyDown(KeyCode.Space))
            InstancierBouleNeigeServerRpc(m_snowballSpawn.position, transform.rotation);    // On laisse le serveur instancier la boule de neige
    }

    [ServerRpc]
    void InstancierBouleNeigeServerRpc(Vector3 p_position, Quaternion p_rotation)
    {
        GameObject snowball = Instantiate(m_snowballPrefab, p_position, p_rotation, null);
        snowball.GetComponent<BouleNeige>().m_id = m_ID;
        snowball.GetComponent<NetworkObject>().Spawn();   // Mettre l'instance de boule de neige sur le serveur pour que le client puisse la voir
    }
}
