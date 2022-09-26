using UnityEngine;
using Unity.Netcode;

public class BouleNeige : NetworkBehaviour
{
    [SerializeField] private float      m_vitesseBouleNeige;
    public NetworkVariable<Vector3>     PositionBouleNeige = new NetworkVariable<Vector3>();

    public int m_id;

    private void Start()
    {
        print("IdBouleNeige = " + m_id);
    }

    void Update()
    {
        DeplacementBouleNeige();
    }

    void DeplacementBouleNeige()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * m_vitesseBouleNeige);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer)
            return;

        if (collision.gameObject.tag == "Player" && m_id != collision.gameObject.GetComponent<PlayerBehavior>().m_ID)
        {
            collision.gameObject.GetComponent<PlayerBehavior>().m_vie -= 10;
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Obstacle")
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Decors")
        {
            Destroy(this.gameObject);
        }
    }
}
