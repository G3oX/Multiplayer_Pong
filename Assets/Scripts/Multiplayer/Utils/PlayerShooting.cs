using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


namespace Multiplayer
{
    public class PlayerShooting : SimulationBehaviour
    {
        // Componentes
        [SerializeField]PlayerScritableObject playerScriptableObject;


        // Start is called before the first frame update
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(Runner.IsServer)
            {
                // Si colisiona una bola entonces la disparamos
                if (collision.gameObject.tag == "Ball")
                {
                    Debug.Log("BALL COLLISION WITH PLAYER");
                    shootBall(getCollisionVector(collision), collision);

                    TurnsManager.Instance.switchTurns();
                }

            }    
        }

        /// <summary>
        /// Devuelve el vector desde nuestro centro al centro del objeto que ha colisionado. Positivo si la colision sucede a la derecha de nustro centro
        /// o Negativo si sucede a la izquierda de nuestro centro
        /// </summary>
        /// <param name="collision"> Collider del objeto que ha colisionado con nosotros</param>
        /// <returns></returns>
        public Vector2 getCollisionVector(Collision2D collision)
        {
            return collision.transform.position - transform.position;
        }

        public void shootBall(Vector2 vCollision, Collision2D collision)
        {
            Vector2 shootDirection = vCollision + playerScriptableObject.shootingVectorCorrection;
            collision.gameObject.GetComponent<PhysxBallMulty>().addForceM(shootDirection.normalized * playerScriptableObject.shootingForce);
        }
    }
}

