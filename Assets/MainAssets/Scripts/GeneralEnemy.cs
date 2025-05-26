using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

namespace Sandbox3D
{
    public class GeneralEnemy : Entity
    {

        public int bodyDamage;
        public float targetRange;
        public Ranged ranged;
        public Vector3 playerRespawnPos;
        public float checkOtherEnemiesRadius;
        public GameObject enemyPrefab;
        public AudioClip ugh;

        float lastShot;
        Quaternion randomRotation;
        float lastRandomRotation = -10;
        float lastJump;
        Rigidbody rb;
        public Vector3 startPos;
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = FindFirstObjectByType<AudioSource>();
            startPos = transform.position;
            rb = GetComponent<Rigidbody>();    
        }

        //private IEnumerator Respawn()
        //{
        //    yield return new WaitForSeconds(2f);
        //    transform.position = startPos;
        //}

        private void Update()
        {
            //if(transform.position.y < -2) { transform.position = startPos; }
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

                        randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

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

            if(transform.position.y >= 0) { return; }

            RaycastHit raycastHit;

            Physics.Raycast(transform.position, transform.forward, out raycastHit, 1);

            if (!raycastHit.collider) { return; }

            if (raycastHit.collider.tag == "Interactable" || raycastHit.collider.tag == "Indestructable") { lastJump = Time.time; Jump(); }

        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!collision.isTrigger)
            {
                if (collision.gameObject.tag == "Player")

                {

                    //transform.position = startPos;
                    collision.gameObject.GetComponent<InputPlayer>().RespawnPlayer();
                    audioSource.PlayOneShot(ugh);

                }

            }

            if (collision.GetComponent<Cube>() != null)
            {
                //collision.GetComponent<Cube>().enabled = true;
                collision.GetComponent<Collider>().isTrigger = false;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Cube>() != null)
            {
                //other.GetComponent<Cube>().enabled = false;
                other.GetComponent<Collider>().isTrigger = true;
            }
        }

        private void OnDrawGizmos()
        {

            Gizmos.DrawRay(transform.position, transform.forward);

        }

        public void KnockBack()
        {
            GameObject player = FindFirstObjectByType<InputPlayer>().gameObject;
            Vector3 hitDirection = (transform.position - player.transform.position).normalized;
            rb.AddForce(hitDirection * 750, ForceMode.Impulse);
        }

        private void OnDestroy()
        {
            //transform.position = startPos;
            //Instantiate(enemyPrefab, startPos, Quaternion.identity);
            FindFirstObjectByType<PlayerInput>().GetComponent<Inventory>();
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
