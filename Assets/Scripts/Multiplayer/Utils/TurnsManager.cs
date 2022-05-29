using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Multiplayer
{
    public class TurnsManager : MonoBehaviour
    {

        private static TurnsManager _instance;

        public static TurnsManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    Debug.LogError("TurnsManager is null");
                }
                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        NetworkPlayer[] players = new NetworkPlayer[2];

        public void switchTurns()
        {
            if(players.Length >= 2)
            {
                if(players[0].myTurn == false)
                {
                    players[0].myTurn = true;
                    players[1].myTurn = false;
                }
                else
                {
                    players[0].myTurn = false;
                    players[1].myTurn = true;
                }
            }
            else
            {
                if (players[0].myTurn == false)
                {
                    players[0].myTurn = true;
                }
                else players[0].myTurn = false;
            }
        }

        public void addPlayer(NetworkPlayer player, int index)
        {
            players[index] = player;
        }

    }
}

