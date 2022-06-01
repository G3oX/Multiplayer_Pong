using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace local
{
    public class RoomManager : MonoBehaviour
    {

        string _roomName;
        [SerializeField] int _maxPlayers = 2;

        public string roomName { get { return _roomName; } set { _roomName = value; } }
        public int maxPlayers { get { return _maxPlayers; } set { _maxPlayers = value; } }

        //public void Awake()
        //{
        //    DontDestroyOnLoad(this.gameObject);
        //}

    }

}
