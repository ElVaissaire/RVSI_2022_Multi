using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehavior : NetworkBehaviour
{

    [SerializeField] private int        m_vitesse = 5;
    [SerializeField] private int        m_rotSpeed;
    [SerializeField] private Vector3    m_direction;
    [SerializeField] private Vector3    m_directionTmp;
    [SerializeField] private Vector3    m_rotation;
    [SerializeField] private Vector3    m_rotCam;

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
        //print(m_cam.transform.rotation);
        if(IsOwner)
        {
            m_directionTmp = GetDirection();
            m_rotation = GetRotation();
            m_rotCam = GetRotCam();
            //if (m_directionTmp != Vector3.zero)
            //{
            m_direction = m_directionTmp;

            if (IsClient && !IsServer)
            {
                EnvoiePositionRotationClientAuServeurServerRpc(m_direction * m_vitesse * Time.deltaTime, m_rotation, m_rotCam); //m_direction
            }
            else
            {
                transform.Translate(m_direction * m_vitesse * Time.deltaTime);
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_rotation/*m_direction*/), 0.3f);
                transform.Rotate(m_rotation);
                //if (m_cam.transform.rotation.x <= 0.25 && m_cam.transform.rotation.x >= -0.1)
                m_cam.transform.Rotate(m_rotCam);
                print(m_cam.transform.eulerAngles.x);

                /*if (45.0f < m_cam.transform.eulerAngles.x && m_cam.transform.eulerAngles.x < 180.0f)
                    m_cam.transform.eulerAngles = new Vector3(45.0f, 0.0f, 0.0f);
                
                if (180.0f < m_cam.transform.eulerAngles.x && m_cam.transform.eulerAngles.x < 330.0f)
                    m_cam.transform.eulerAngles = new Vector3(330.0f, 0.0f, 0.0f);*/
                /*
                if (m_cam.transform.eulerAngles.x < -45.0f)
                    m_cam.transform.eulerAngles = new Vector3(-45.0f, 0.0f, 0.0f);

                if (m_cam.transform.eulerAngles.x > 45.0f)
                    m_cam.transform.eulerAngles = new Vector3(45.0f, 0.0f, 0.0f);//*/

                //else if(m_cam.transform.rotation.x > 0.25 && m_cam.transform.rotation.x < 0)
                //    m_cam.transform.Rotate(m_rotCam);
                //else if (m_cam.transform.rotation.eulerAngles.x < -0.1 && m_cam.transform.rotation.eulerAngles.x < -0.1) 
            }

            LancerBouleNeige();
        }

        m_vie = m_networkVie.Value;
        m_healthBar.value = m_vie;

    }
    
    [ServerRpc]
    void EnvoiePositionRotationClientAuServeurServerRpc(Vector3 p_direction, Vector3 p_rotation, Vector3 p_rotCam)
    {
        transform.Translate(p_direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(p_rotation), 0.3f);
        transform.Rotate(p_rotation);
        m_cam.transform.Rotate(p_rotCam);
    }

    Vector3 GetRotCam()
    {
        float axisY = Input.GetAxis("Mouse Y");
        Vector3 rotation = new Vector3(-axisY * Time.deltaTime * m_rotSpeed, 0, 0);
        return rotation;
    }

    Vector3 GetRotation()
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
