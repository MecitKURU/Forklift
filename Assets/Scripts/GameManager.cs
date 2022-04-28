using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public int enteringBoxCount;
    [SerializeField] GameObject winPanel;

    private void Awake()
    {
        _instance = this;
    }
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("GameManager is Null!");
            }

            return _instance;
        }
    }


    public void Fail()
    {
        SceneManager.LoadScene(0);
    }

    public void Win()
    {
        winPanel.SetActive(true);
    }
    public void Continue()
    {
        SceneManager.LoadScene(0);
    }

}
