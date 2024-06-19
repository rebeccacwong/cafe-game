using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// The GameManager is a Singleton object that manages the game
/// lifecycle, and information that needs to be retained
/// across multiple days/rounds of the game.
/// 
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Game data
    private float m_playerMoney;
    #endregion

    private UI _cc_UI;
    private int _level = 0;

    private Dictionary<int, Level> _levels = new Dictionary<int, Level>();

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

        _levels.Add(1, new LevelOne());
        _levels.Add(2, new LevelTwo());
        _levels.Add(3, new LevelThree());

        DontDestroyOnLoad(this.gameObject);
    }

    public float getPlayerMoneyAmount()
    {
        return m_playerMoney;
    }

    public void addToPlayerMoneyAmount(float toAdd)
    {
        Debug.Assert(toAdd >= 0);
        m_playerMoney += toAdd;
        if (!_cc_UI)
        {
            _cc_UI = GameObject.Find("Canvas").GetComponent<UI>();
            Debug.Assert(_cc_UI != null);
        }
        Debug.LogFormat("Added {0} dollars to player money amount.", toAdd);
        _cc_UI.updateMoneyUI();
    }

    public void subtractFromPlayerMoneyAmount(float toSubtract)
    {
        Debug.Assert(toSubtract >= 0);
        m_playerMoney -= toSubtract;
        if (!_cc_UI)
        {
            _cc_UI = GameObject.Find("Canvas").GetComponent<UI>();
            Debug.Assert(_cc_UI != null);
        }
        Debug.LogFormat("Subtracted {0} dollars to player money amount.", toSubtract);
        _cc_UI.updateMoneyUI();
    }

    /*
     * Overrides the amount of money that the player 
     * has to be moneyAmount.
     */
    public void setPlayerMoneyAmount(float moneyAmount)
    {
        m_playerMoney = moneyAmount;
    }

    /*
     * Sets up the necessary variables for the next level.
     * Returns the level that we are on.
     */
    public int InitializeNewLevel()
    {
        if (_level == 3)
        {
            // Game over
            // TODO: display a game complete screen
            return _level;
        }

        _level++;
        _levels[_level].InitializeLevel();
        return _level;
    }
}