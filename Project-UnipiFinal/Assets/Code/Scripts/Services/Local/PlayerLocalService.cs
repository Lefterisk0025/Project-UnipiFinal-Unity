using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerLocalService
{
    public bool CreatePlayer(string displayName, int gender)
    {
        try
        {
            PlayerPrefs.SetString("DisplayName", displayName);
            PlayerPrefs.SetInt("Gender", gender);
            PlayerPrefs.SetInt("Reputation", 0);
            PlayerPrefs.SetInt("NetCoins", 0);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("An error occured whule creating local Player, " + e.Message);
            return false;
        }
    }

    public Player LoadPlayer()
    {
        try
        {
            string tempDisplayName = PlayerPrefs.GetString("DisplayName");
            if (tempDisplayName == null || tempDisplayName.Equals(""))
                return null;

            return new Player()
            {
                DisplayName = tempDisplayName,
                Gender = PlayerPrefs.GetInt("Gender"),
                Reputation = PlayerPrefs.GetInt("Reputation"),
                NetCoins = PlayerPrefs.GetInt("NetCoins"),
            };
        }
        catch (Exception e)
        {
            Debug.Log("An error occured whule creating local Player, " + e.Message);
            return null;
        }
    }

    public void UpdatePlayer(Player player)
    {
        try
        {
            PlayerPrefs.SetString("DisplayName", player.DisplayName);
            PlayerPrefs.SetInt("Gender", player.Gender);
            PlayerPrefs.SetInt("Reputation", player.Reputation);
            PlayerPrefs.SetInt("NetCoins", player.NetCoins);
        }
        catch (Exception e)
        {
            Debug.Log("An error occured whule updating local Player, " + e.Message);
            return;
        }
    }

    public bool DeletePlayer()
    {
        try
        {
            PlayerPrefs.SetString("DisplayName", "");
            PlayerPrefs.SetInt("Gender", 0);
            PlayerPrefs.SetInt("Reputation", 0);
            PlayerPrefs.SetInt("NetCoins", 0);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log("An error occured whule deleting local Player, " + e.Message);
            return false;
        }
    }
}
