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
            //players = new List<NetworkPlayer>();
        }

        List<NetworkPlayer> players = new List<NetworkPlayer>();

        public void switchTurns()
        {
            if(players.Count > 1)
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

        public void addPlayer(NetworkPlayer player)
        {
            players.Add(player);
        }

    }
}

