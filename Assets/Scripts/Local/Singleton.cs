using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace local
{
    public class Singleton<T> : MonoBehaviour where T : class
    {
        // Start is called before the first frame update
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("No se encuentra la instancia");
                }
                return _instance;
            }
        }

    }
}

