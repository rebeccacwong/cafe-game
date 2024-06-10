using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The GameManager is a Singleton object that manages the game
/// lifecycle, and information that needs to be retained
/// across multiple days/rounds of the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Game data
    private float m_playerMoney;
    #endregion

    private UI _cc_UI;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        Debug.Log("Initialized GameManager");

        _cc_UI = GameObject.Find("Canvas").GetComponent<UI>();
        DontDestroyOnLoad(this.gameObject);
    }

    public float getPlayerMoneyAmount()
    {
        return m_playerMoney;
    }

    public void addToPlayerMoneyAmount(float toAdd)
    {
        m_playerMoney += toAdd;
        if (_cc_UI)
        {
            Debug.LogFormat("Added {0} dollars to player money amount.", toAdd);
            _cc_UI.updateMoneyUI();
        }
    }

    /*
     * Overrides the amount of money that the player 
     * has to be moneyAmount.
     */
    public void setPlayerMoneyAmount(float moneyAmount)
    {
        m_playerMoney = moneyAmount;
    }
}