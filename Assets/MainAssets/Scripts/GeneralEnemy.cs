using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Sandbox3D
{
    public class GeneralEnemy : Entity
    {

        public int bodyDamage;
        public float targetRange;
        public Ranged ranged;

        float lastShot;
        Quaternion randomRotation;
        float lastRandomRotation = -10;
        float lastJump;

        private void Update()
        {

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player)
            {

                if (Vector3.Distance(player.transform.position, transform.position) < range)
                {

                    if (Vector3.Distance(player.transform.position, transform.position) > targetRange)
                    {

                        Vector3 target = player.transform.position - transform.position;

                        target.y = 0;

                        transform.rotation = Quaternion.LookRotation(target, Vector3.up);

                        transform.Translate(Vector3.forward * Time.deltaTime * speed);

                        CheckJump();

                    }


                    if (Vector3.Distance(player.transform.position, transform.position) < ranged.shotRange)
                    {

                        if (Time.time > lastShot + ranged.shotInterval)
                        {

                            lastShot = Time.time;

                            Debug.Log("Shoot at the player");

                            if (ranged.shotPrefab)
                            {

                                GameObject shot = Instantiate(ranged.shotPrefab, transform.position, Quaternion.identity);

                                shot.transform.rotation = Quaternion.LookRotation(player.transform.position - shot.transform.position);

                            }

                        }

                    }

                }
                else
                {

                    if (Time.time > lastRandomRotation + 10)
                    {

                        lastRandomRotation = Time.time;

                        randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    }

                    if (Time.time < lastRandomRotation + 6)
                    {

                        transform.rotation = randomRotation;

                        transform.Translate(Vector3.forward * Time.deltaTime * speed);

                        CheckJump();

                    }

                }

            }

        }

        void CheckJump()
        {

            if (Time.time < lastJump + 1) { return; }

            RaycastHit raycastHit;

            Physics.Raycast(transform.position, transform.forward, out raycastHit, 1);

            if (!raycastHit.collider) { return; }

            if (raycastHit.collider.tag == "Interactable") { lastJump = Time.time; Jump(); }

        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.gameObject.tag == "Player") { Debug.Log("Player hit"); }

            // Suggestion: Decrease the player's health inherited from Entity,
            // Once it reaches zero - call Entity.Die() that is overitten by the player to
            // display the death screen

        }

        private void OnDrawGizmos()
        {

            Gizmos.DrawRay(transform.position, transform.forward);

        }

    }


    [System.Serializable]
    public struct Ranged
    {

        public float shotRange;
        public float shotInterval;
        public GameObject shotPrefab;

    }


}
