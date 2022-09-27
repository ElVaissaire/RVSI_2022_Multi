using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    private GameManager() { }
    public static GameManager Instance { get; private set; }

    private GameObject[] m_playerArray = new GameObject[8];
    public Color[] m_playerColor;
    [SerializeField] private Canvas m_gameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        else Instance = this;

        for (int i = 0; i < 8; i++)
            m_playerArray[i] = null;
    }

    public int AddPlayer(GameObject p_player)
    {
        int i;

        for (i = 0; i < 8; i++)
        {
            if (!m_playerArray[i])
            {
                m_playerArray[i] = p_player;
                break;
            }
        }

        if (i == 8)
            return -1;

        p_player.GetComponent<MeshRenderer>().material.color = m_playerColor[i];

        return i;
    }

    public Color GetColorByID(int p_id)
    {
        if (p_id < 0 || p_id >= 8)
            return Color.black;
        else
            return m_playerColor[p_id];
    }

    public void DeletePlayer(int p_id)
    {
        m_playerArray[p_id] = null;
    }

    public GameObject GetPlayer(int p_id)
    {
        if (Mathf.Abs(p_id) > 8)
            return null;

        return m_playerArray[Mathf.Abs(p_id)];
    }

   

    public void GameOver()
    {
        m_gameOver.gameObject.SetActive(true);
    }
    public void PlayAgain()
    {
       m_gameOver.gameObject.SetActive(false);
       for (int i = 0; i < 8; i++)
       {
            if (m_playerArray[i] != null)
            {
                m_playerArray[i].GetComponent<PlayerBehavior>().Respawn();
            }
       }
        
    }

    public void Quit()
    {
        Application.Quit();
    }
}
