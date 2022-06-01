using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;


namespace Multiplayer
{
    public class TurnsManager : SimulationBehaviour
    {
        //Variables

        List<NetworkPlayer> players = new List<NetworkPlayer>();

        #region SINGLETON

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

        #endregion

        private void Awake()
        {
            _instance = this;           
        }

        public void switchTurns()
        {

            if (players.Count > 1)
                switchTurns2players();
            else
                switchTurns1player();
        }

        public void switchTurns2players()
        {
            if (players[0].myTurn == false)
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

        public void switchTurns1player()
        {
            if (players[0].myTurn == false) 
                players[0].myTurn = true;
            else
                players[0].myTurn = false;
        }
            
        public void addPlayer(NetworkPlayer player)
        {
            players.Add(player);
        }

    }
}

