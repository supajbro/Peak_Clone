using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChibiCharacter
{


    public class CharacterMover : MonoBehaviour
    {
        #region variables
        [Header("Player Stats")]
        private float jumpForce = 12;

        [SerializeField]
        private float health = 3;

        [SerializeField]
        private float moveSpeed = 5;

        [SerializeField]
        private float runSpeedMultiplier = 1.5f;

        [SerializeField]
        private float groundedDistance = 1;

        [SerializeField]
        [Tooltip("After getting hurt, how long is player invincible before they can get hurt again")]
        private float hurtInvincibilityTime = 0.5f;

        [Header("Object references")]
        [SerializeField]
        private GameObject interactCanvas;

        [SerializeField]
        private ParticleSystem victoryParticles;

        [SerializeField]
        private Text healthText;

        [SerializeField]
        private GameObject deadPanel;

        [SerializeField]
        private GameObject bomb;

        [SerializeField]
        private Animator anim;


        private Rigidbody rb;
        private Collider col;
        private AudioSource audioSource;

        [Header("Audio clips")]
        [SerializeField]
        private AudioClip jumpSound;

        [SerializeField]
        private AudioClip hurtSound;

        [SerializeField]
        private AudioClip dieSound;

        [SerializeField]
        private AudioClip successSound;

        [Header("Debug")]
        [SerializeField]
        private Vector2 velocity;

        [SerializeField]
        [Tooltip("The interactable object that player is currently near")]
        private InteractableObject interactableObject;

        private DoubleTapDetector doubleTaps;

        [SerializeField]
        private bool isRunning;

        [SerializeField]
        private bool isInWater;

        //whether in water in the previous frame
        /// <summary>
        /// Is one frame behind the isInWater
        /// If isInWater and lastInWater are opposite then player has either just gotten in the water or just gotten out
        private bool lastInWater;

        private bool inInteractZone;

        private bool hasDoubleJumped = false;

        private bool isClimbing;

        //the timestamp where player last took damage
        private float lastHurtTime = 0;

        //whether player is facing right
        private bool isFacingRight;

        private bool levelFinished;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            //add listener for detecting double taps of left or right arrow
            doubleTaps = new DoubleTapDetector();
            doubleTaps.Add(KeyCode.RightArrow);
            doubleTaps.Add(KeyCode.LeftArrow);

            rb = gameObject.GetComponent<Rigidbody>();
            col = gameObject.GetComponent<Collider>();
            audioSource = gameObject.GetComponent<AudioSource>();

            interactCanvas.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            velocity = rb.velocity;

            if (health > 0)
            {
                if (levelFinished == false)
                {
                    anim.SetBool("isDead", false);
                    anim.SetBool("swimming", lastInWater);

                    //basic grounded movement
                    if (isInWater == false && isClimbing == false)
                    {
                        //if I just got out of the water in this frame
                        if (lastInWater)
                        {
                            hasDoubleJumped = false;

                            //reset the rotation from the water
                            if (velocity.x >= 0)
                            {
                                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                                isFacingRight = true;
                            }
                            else
                            {
                                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                                isFacingRight = false;
                            }

                            //if you are jumping out of the water
                            if (Input.GetKey(KeyCode.UpArrow))
                            {
                                velocity.y = jumpForce;
                                anim.SetBool("isGrounded", true);
                                anim.SetBool("isJumping", true);
                            }

                        }

                        groundMovements();

                        //see if bomb can be thrown
                        if (Input.GetKeyDown(KeyCode.B))
                        {
                            Debug.Log("throw bomb");
                            StartCoroutine(waitBeforeThrowingBomb());

                        }

                        RaycastHit? ladderRay = shootLadderRay(Vector3.forward, 5);
                        if (ladderRay != null && Input.GetKey(KeyCode.UpArrow))
                        {
                            isClimbing = true;
                            anim.SetBool("isClimbing", isClimbing);
                        }


                    }
                    else if (isInWater && isClimbing == false)
                    {
                        waterMovements();
                    }
                    else if (isClimbing)
                    {
                        climbingMovements();

                        RaycastHit? ladderRay = shootLadderRay(Vector3.forward, 5);
                        if (ladderRay == null)
                        {
                            isClimbing = false;
                            anim.SetBool("isClimbing", isClimbing);

                            hasDoubleJumped = false;

                            //if you are jumping off the ladder
                            if (Input.GetKey(KeyCode.UpArrow))
                            {
                                velocity.y = jumpForce;
                                anim.SetBool("isGrounded", true);
                                anim.SetBool("isJumping", true);
                            }
                        }
                    }

                    if (inInteractZone && Input.GetKeyDown(KeyCode.Space) && velocity.y >= -1)
                    {
                        Debug.Log("interact");
                        anim.SetTrigger(interactableObject.getAnimationTriggerName());
                        interactableObject.interact();
                    }

                }

            }
            else
            {
                //if I just became dead
                if (anim.GetBool("isDead") == false)
                {
                    anim.SetBool("isDead", true);
                    velocity.x = 0;
                    deadPanel.SetActive(true);
                    audioSource.clip = dieSound;
                    audioSource.Play();
                }

            }

            anim.SetFloat("yVelocity", velocity.y);
            //apply the velocity
            rb.velocity = velocity;
            lastInWater = isInWater;

            healthText.text = "Health: " + health;
        }

        /// <summary>
        /// Brings the player back to life
        /// </summary>
        public void revive()
        {
            //bring character near lava
            gameObject.transform.position = new Vector3(100, 32, 0);

            deadPanel.SetActive(false);
            StartCoroutine(waitBeforeReviving());
        }

        IEnumerator waitBeforeReviving()
        {
            yield return new WaitForSeconds(2);

            //put health back where it belongs
            health = 3;

            anim.SetBool("isDead", false);
        }


        IEnumerator waitBeforeThrowingBomb()
        {
            anim.SetTrigger("throwBomb");
            yield return new WaitForSeconds(0.5f);
            GameObject currentBomb = Instantiate(bomb);
            if (isFacingRight)
            {
                currentBomb.transform.position = gameObject.transform.position + new Vector3(2, 3, 0);
                currentBomb.GetComponent<Rigidbody>().velocity = new Vector3(15, 5);
            }
            else
            {
                currentBomb.transform.position = gameObject.transform.position + new Vector3(-2, 3, 0);
                currentBomb.GetComponent<Rigidbody>().velocity = new Vector3(-15, 5);
            }

        }

        /// <summary>
        /// Damages the player
        /// </summary>
        public void damagePlayer()
        {
            //only damage player if player is not invincible
            if (Time.time - lastHurtTime > hurtInvincibilityTime && health > 0)
            {
                lastHurtTime = Time.time;
                anim.SetTrigger("hurt");
                health -= 1;
                audioSource.clip = hurtSound;
                audioSource.Play();
            }

        }


        /// <summary>
        /// Basic ground locomotion (not in water and not on ladder)
        /// </summary>
        private void groundMovements()
        {
            rb.useGravity = true;

            //detecting running
            #region determines whether running or not
            if (doubleTaps.GetKeyDoubleTap(KeyCode.RightArrow))
            {
                isRunning = true;
            }
            if (doubleTaps.GetKeyDoubleTap(KeyCode.LeftArrow))
            {
                isRunning = true;
            }

            //if the right key was relesed and the left arrow is not being held, then cancel the run
            if (Input.GetKeyUp(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow) == false)
            {
                isRunning = false;
            }

            //if the left key was relesed and the right arrow is not being held, then cancel the run
            if (Input.GetKeyUp(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) == false)
            {
                isRunning = false;
            }
            #endregion

            if (Input.GetKey(KeyCode.RightArrow))
            {
                velocity.x = moveSpeed;
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                isFacingRight = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                velocity.x = -moveSpeed;
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                isFacingRight = false;
            }
            else
            {
                velocity.x = 0;
            }

            if (isRunning)
            {
                velocity.x *= runSpeedMultiplier;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (isGrounded())
                {
                    velocity.y = jumpForce;
                    audioSource.clip = jumpSound;
                    audioSource.Play();
                    anim.SetBool("isJumping", true);
                }
                else if (hasDoubleJumped == false && anim.GetBool("isJumping") == false)
                {
                    velocity.y = 8;
                    audioSource.clip = jumpSound;
                    audioSource.Play();
                    anim.SetBool("isJumping", true);
                    hasDoubleJumped = true;
                }
            }

            if (isGrounded())
            {
                hasDoubleJumped = false;
            }

            anim.SetBool("isGrounded", isGrounded());

            anim.SetFloat("moveSpeed", (Mathf.Abs(velocity.x) / moveSpeed) / runSpeedMultiplier);
        }

        /// <summary>
        /// The movement system for when in the water
        /// </summary>
        private void waterMovements()
        {
            rb.useGravity = false;

            if (Input.GetKey(KeyCode.RightArrow))
            {
                velocity = velocity.normalized * moveSpeed;
                velocity.x = moveSpeed;
                isFacingRight = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                velocity.x = -moveSpeed;
                velocity = velocity.normalized * moveSpeed;
                isFacingRight = false;
            }
            else
            {
                //stopping resistance in water is less
                if (velocity.x > 0)
                {
                    velocity.x -= Time.deltaTime * 10;
                }
                else if (velocity.x < 0)
                {
                    velocity.x += Time.deltaTime * 10;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                velocity.y = moveSpeed;
                velocity = velocity.normalized * moveSpeed;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                velocity.y = -moveSpeed;
                velocity = velocity.normalized * moveSpeed;
            }
            else
            {
                //stopping resistance in water is less
                if (velocity.y > 0)
                {
                    velocity.y -= Time.deltaTime * 10;
                }
                else if (velocity.y < 0)
                {
                    velocity.y += Time.deltaTime * 10;
                }
            }

            //slowly rotate in the direction of movement
            if (velocity.magnitude / moveSpeed > 0.1f)
            {
                //have to add a little bit of a vector to prevent unpredictable rotation when facing completely up or down
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(velocity.normalized + new Vector2(0.1f, 0)), 0.2f);
            }

            //the moveSpeed during swim should not go below 0.3 (so the treading water animation does not go too slow)
            anim.SetFloat("moveSpeed", Mathf.Max((velocity.magnitude / moveSpeed), 0.3f));
        }

        /// <summary>
        /// Movement system when on a ladder
        /// </summary>
        private void climbingMovements()
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 359, 0));
            rb.useGravity = false;
            velocity.x = 0;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                velocity.y = moveSpeed;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                velocity.y = -moveSpeed;
            }
            else
            {
                velocity.y = 0;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                velocity.x = moveSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                velocity.x = -moveSpeed;
            }
            else
            {
                velocity.x = 0;
            }

            anim.SetFloat("moveSpeed", velocity.y / moveSpeed);

            rb.velocity = velocity;
        }

        private bool isGrounded()
        {
            Vector3 centerOrigin = gameObject.transform.position + new Vector3(0, 0.1f, 0);

            RaycastHit? centerRay = shootRay(centerOrigin, Vector3.down, groundedDistance);
            RaycastHit? rightRay = shootRay(centerOrigin + new Vector3(col.bounds.size.x / 2f, 0, 0), Vector3.down, groundedDistance);
            RaycastHit? leftRay = shootRay(centerOrigin + new Vector3(-col.bounds.size.x / 2f, 0, 0), Vector3.down, groundedDistance);

            //returns true if any of the rays hit a solid collider, false otherwise
            return (centerRay != null || rightRay != null || leftRay != null);
        }

        /// <summary>
        /// Returns an array if it hit a solid collider, otherwise returns null
        /// </summary>
        /// <param name="pos">The origin position to shoot ray from</param>
        /// <returns></returns>
        private RaycastHit? shootRay(Vector3 pos, Vector3 direction, float distance)
        {
            RaycastHit[] rays = Physics.RaycastAll(new Ray(pos, direction * distance), distance);
            if (rays.Length > 0)
            {
                foreach (RaycastHit ray in rays)
                {
                    //only count a ray if it is a solid object
                    if (ray.collider.isTrigger == false)
                    {
                        Debug.DrawRay(pos, direction * distance, Color.green);
                        return ray;
                    }
                }
            }

            Debug.DrawRay(pos, direction * distance, Color.red);
            return null;
        }

        /// <summary>
        /// Returns a ray if it hit a solid collider that has the given script on it, otherwise returns null
        /// </summary>
        /// <param name="pos">The origin position to shoot ray from</param>
        /// <returns></returns>
        private RaycastHit? shootLadderRay(Vector3 direction, float distance)
        {
            Vector2 pos = gameObject.transform.position + new Vector3(0, 0.1f, 0);

            RaycastHit[] rays = Physics.RaycastAll(new Ray(pos, direction * distance), distance);
            if (rays.Length > 0)
            {
                foreach (RaycastHit ray in rays)
                {
                    //only count a ray if it is a solid object that hit the object with the requiredScript on it
                    if (ray.collider.isTrigger == false && ray.collider.gameObject.GetComponent<Ladder>() != null)
                    {
                        Debug.DrawRay(pos, direction * distance, Color.green);
                        return ray;
                    }
                }
            }

            Debug.DrawRay(pos, direction * distance, Color.red);
            return null;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<InteractableObject>() != null)
            {
                interactCanvas.SetActive(true);
                inInteractZone = true;
                interactableObject = other.gameObject.GetComponent<InteractableObject>();
            }
            else if (other.gameObject.name == "Water")
            {
                isInWater = true;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<InteractableObject>() != null)
            {
                inInteractZone = false;
                interactCanvas.SetActive(false);
                interactableObject = null;
            }
            else if (other.gameObject.name == "Water")
            {
                isInWater = false;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.name == "Lava")
            {
                damagePlayer();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //if player collided with the finishing platform then end the level
            if (collision.gameObject.name == "Victory")
            {
                victoryParticles.Play();
                levelFinished = true;
                velocity = Vector2.zero;
                gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                anim.SetBool("victory", true);
                anim.SetBool("isGrounded", true);
                audioSource.clip = successSound;
                audioSource.Play();
            }
        }

    }
}
