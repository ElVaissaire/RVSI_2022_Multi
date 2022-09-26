using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehavior : NetworkBehaviour
{

    [SerializeField] private int        m_vitesse = 5;
    [SerializeField] private int        m_rotSpeed;
    [SerializeField] private Vector3    m_direction;
    [SerializeField] private Vector3    m_rotation;
    [SerializeField] private Vector3    m_rotCam;
    public NetworkVariable<Vector3>     m_networkRotCam;
    public NetworkVariable<Quaternion>  m_networkRotationCam;

    [SerializeField] private GameObject m_snowballPrefab;
    [SerializeField] private Transform  m_snowballSpawn;

    [SerializeField] public Slider      m_healthBar;
    public NetworkVariable<int>         m_networkVie = new NetworkVariable<int>();
    public static int                   m_maxVie = 5;
    [SerializeField] int                m_vie;

    private NetworkVariable<int>        m_networkID = new NetworkVariable<int>();
    public int                          m_ID = -1;

    [SerializeField] private GameObject m_cam;

    [SerializeField] public TextMeshProUGUI m_text;
    //public NetworkVariable<string> m_networkText = new NetworkVariable<string>();

    private void Awake()
    {
        m_ID = -1;
        m_healthBar.maxValue = m_maxVie;
        m_vie = m_maxVie;
        //m_text.text = "Player";
        m_text.SetText("Player");
        m_text.fontSize = 5;
        print("Awake " + m_text.text);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            m_ID = GameManager.Instance.AddPlayer(gameObject);
            m_networkID.Value = m_ID;
            m_networkVie.Value = m_maxVie;
            //m_text.text = "Player" + m_ID.ToString();
            //m_text.fontSize = 5;

            print("OnNetworkSpawn " + m_text.text);
        }
        else
        {
            m_ID = m_networkID.Value;
            m_maxVie = m_networkVie.Value;
            GetComponent<MeshRenderer>().material.color = GameManager.Instance.GetColorByID(m_ID);
            //m_text.text = "Player";
        }

        if (!IsOwner)
            m_cam.SetActive(false);
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
            m_direction = GetDirection();
            m_rotation = GetRotationPersonnage();
            m_rotCam = GetRotationCamera();

            if (IsServer)
            {
                transform.Translate(m_direction * m_vitesse * Time.deltaTime);
                transform.Rotate(m_rotation);

                // Autoriser la rotation entre 330° et 45°
                if ((0.0f <= m_cam.transform.eulerAngles.x && m_cam.transform.eulerAngles.x < (45.0f - m_rotCam.x) ) || 
                    (((330.0f - m_rotCam.x) < m_cam.transform.eulerAngles.x && m_cam.transform.eulerAngles.x <= 360.0f)))
                {
                    m_cam.transform.Rotate(m_rotCam);
                }
                
                m_networkRotationCam.Value = m_cam.transform.rotation;
            }
            else
            {
                EnvoiePositionRotationClientAuServeurServerRpc(m_direction * m_vitesse * Time.deltaTime, m_rotation, m_rotCam); //m_direction
            }

            LancerBouleNeige();
        }

        m_vie = m_networkVie.Value;
        m_healthBar.value = m_vie;
        m_cam.transform.rotation = m_networkRotationCam.Value;
    }
    
    [ServerRpc]
    void EnvoiePositionRotationClientAuServeurServerRpc(Vector3 p_direction, Vector3 p_rotation, Vector3 p_rotCam)
    {
        transform.Translate(p_direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(p_rotation), 0.3f);
        transform.Rotate(p_rotation);
        //m_cam.transform.Rotate(p_rotCam);
    }

    Vector3 GetRotationCamera()     // Tourner uniquement la caméra pour regarder de haut en bas
    {
        float axisY = Input.GetAxis("Mouse Y");
        Vector3 rotation = new Vector3(-axisY * Time.deltaTime * m_rotSpeed, 0, 0);
        return rotation;
    }

    Vector3 GetRotationPersonnage()     // Tourner le personnage autour de l'axe y pour regarder sur les côtes
    {
        Vector3 rotation = new Vector3(0, Input.GetAxis("Mouse X") * Time.deltaTime * m_rotSpeed, 0);
        return rotation;
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
