using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] lifeSymbols;
    public Text coinText;

    public void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeSymbols.Length; i++)
        {
            if (lives > i)
                lifeSymbols[i].color = Color.white;
            else
            {
                lifeSymbols[i].color = Color.black;
            }
        }
    }

    public void UpdateCoins(int coin)
    {
        coinText.text = coin.ToString();
    }
}
