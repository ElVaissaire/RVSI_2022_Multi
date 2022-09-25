using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BouleNeige : NetworkBehaviour
{
    [SerializeField] public float       m_vitesseBouleNeige;

    [SerializeField] public Vector3     m_direction;

    public NetworkVariable<Vector3> PositionBouleNeige = new NetworkVariable<Vector3>();

    void Update()
    {
        DeplacementBouleNeige();
    }

    void DeplacementBouleNeige()
    {
        transform.Translate(m_direction * Time.deltaTime * m_vitesseBouleNeige);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //print("Player");
            PlayerBehavior.player_S.m_score -= 10;     
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
