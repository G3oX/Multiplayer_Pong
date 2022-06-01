using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace networking
{
    public class BallSpawner : NetworkBehaviour
    {
        [SerializeField] NetworkPrefabRef _prefabPhysxBall;
        [SerializeField] float timeToSpawn = 2f;

        private NetworkRunner _runner;
        [SerializeField] BasicSpawner spawner;
        [Networked] private TickTimer delay { get; set; }



        public void Awake()
        {
            _runner = spawner.getRunner();
        }

        public override void FixedUpdateNetwork()
        {

            if(delay.ExpiredOrNotRunning(_runner))
            {
                delay = TickTimer.CreateFromSeconds(_runner, timeToSpawn);
                _runner.Spawn(_prefabPhysxBall,
                    transform.position,
                    Quaternion.identity,
                    Object.InputAuthority,
                    (_runner, o) =>
                    {
                         // Initialize the Ball before synchronizing it
                         //o.GetComponent<PhysxBall>().Init();
                    });
            }          
        }
    }
}
