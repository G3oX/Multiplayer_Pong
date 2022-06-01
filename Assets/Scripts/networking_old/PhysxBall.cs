using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace networking
{
    public class PhysxBall : NetworkBehaviour
    {
        [SerializeField] string scoreColliderTag = "ScoreCollider";

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == scoreColliderTag)
            {
                Deactivate();
            }
        }
    }

}
