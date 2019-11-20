using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPlatformerController : PhysicsObject
{
    //player attributes
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    private float slideSpeed = 4f;
    private float slideLength = 0.07f;
    public int controllerNumber = 0;
    public bool flippedRight = true;
    public int lives = 3;

    //relevant for item scripts
    public int switched = 1;
    public int flipped = 1;

    //controls
    public string inputJump;
    public string inputSlide;
    public string inputAction;
    public string inputHorizontal;
    public string inputVertical;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private ItemPickup ipReference;

    //UI-Stuff
    private GameObject uiWidgetRef;
    private Image healthIconRef;
    private Image itemIconRef;
    public Sprite playerTag;
    public Sprite DeathTag;
    public List<Sprite> healthIcons;
    public SpriteRenderer itemSlot;

    //respawn and health stuff
    private bool isGettingRespawned = false;
    private GameObject mainCamera;
    private MultipleTargetCamera mainCameraScript;
    private GM_Main gameModeReference;

    private GameObject main;
    private GM_Main mainScript;
    private List<GameObject> players;

    // Initialization
    void Awake()
    {
        inputJump = "P" + controllerNumber + "Jump";
        inputSlide = "P" + controllerNumber + "Slide";
        inputAction = "P" + controllerNumber + "Action";
        inputHorizontal = "P" + controllerNumber + "Horizontal";
        inputVertical = "P" + controllerNumber + "Vertical";

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        ipReference = GetComponent<ItemPickup>();

        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCameraScript = (MultipleTargetCamera)mainCamera.GetComponent(typeof(MultipleTargetCamera));

        /*
        main = GameObject.FindWithTag("GameController");
        mainScript = (GM_Main)main.GetComponent(typeof(GM_Main));
        players = mainScript.getPlayers();
        */
        gameModeReference = FindObjectOfType<GM_Main>();
    }

    //Update
    protected override void ComputeVelocity()
    {

        Vector2 move = Vector2.zero;

        //is controller assigned?
        if (controllerNumber > 0)
        {
            move.x = Input.GetAxis(inputHorizontal);

            if (Input.GetAxis(inputVertical) >= 1)
            {
                Debug.Log("throw!");
                if (ipReference)
                {
                    ipReference.ThrowAway();
                }
                else
                {
                    Debug.LogError("Throw away failed for " + name + "! Missing Reference to ItemPickup component!");
                }
            }
            else if (Input.GetButtonDown(inputAction))
            {
                ipReference.item.action(ipReference.game);
                Debug.Log("action");
            }
            else if (Input.GetAxis(inputVertical) <= -1)
            {
                Debug.Log("block");
            }

            if (grounded && !isSliding && Input.GetButtonDown(inputSlide) && velocity.x != 0)
            {
                slide();
            }
            else if (Input.GetButtonDown(inputJump) && grounded)
            {
                velocity.y = jumpTakeOffSpeed;
                //Debug.Log(inputJump);
            }
            else if (Input.GetButtonUp(inputJump))
            {
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * 0.5f;
                }
            }
        }
        else
        {
            Debug.LogWarning("Controller Number for " + name + " does not exist!");
        }

        //get the current scale of the object
        Vector3 currentScale = transform.localScale;

        //flips the character acoording to the move input
        if (move.x == 0)
        {
            //Do not delete this condition, it is important!
            transform.localScale = currentScale;
        }
        else if (move.x > 0.01f && currentScale.x < 0)
        {
            currentScale.x *= -1;
            flipped *= -1;
            transform.localScale = currentScale;
            flippedRight = true;
        }
        else if (move.x < 0.01f && currentScale.x > 0)
        {
            currentScale.x *= -1;
            flipped *= -1;
            transform.localScale = currentScale;
            flippedRight = false;
        }

        animator.SetBool("grounded", grounded);
        animator.SetBool("isSliding", isSliding);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
        animator.SetFloat("velocityY", Mathf.Abs(velocity.y) / jumpTakeOffSpeed);

        targetVelocity = move * switched * maxSpeed;
    }

    public void setControllerInputs(int controllerNumber)
    {
        this.controllerNumber = controllerNumber;
        inputJump = "P" + controllerNumber + "Jump";
        inputSlide = "P" + controllerNumber + "Slide";
        inputAction = "P" + controllerNumber + "Action";
        inputHorizontal = "P" + controllerNumber + "Horizontal";
        inputVertical = "P" + controllerNumber + "Vertical";
    }

    public void bindUIWidget(GameObject uiWidget)
    {
        uiWidgetRef = uiWidget;
        uiWidgetRef.GetComponent<Image>().sprite = playerTag;
        healthIconRef = uiWidget.transform.Find("HealthIcon").GetComponent<Image>();
        itemIconRef = uiWidget.transform.Find("ItemIcon").GetComponent<Image>();

        healthIconRef.sprite = healthIcons[2];
        healthIconRef.color = new Color(255f, 255f, 255f, 1f);

        updateUI();
    }

    public void updateUI()
    {
        if (lives > 0)
        {
            healthIconRef.sprite = healthIcons[lives - 1];

            if (GetComponentInParent<ItemPickup>().item != null)
            {
                //Debug.Log("item set!");
                itemIconRef.sprite = itemSlot.sprite;
                itemIconRef.color = new Color(255f, 255f, 255f, 1f);
                itemIconRef.type = Image.Type.Simple;
                itemIconRef.preserveAspect = true;

                //if item has type "melee"
                if (GetComponentInParent<ItemPickup>().item.type == 0)
                {
                    if (itemIconRef.rectTransform.localRotation.eulerAngles.z == 0f)
                    {
                        itemIconRef.rectTransform.Rotate(0, 0, 90f);
                        itemIconRef.rectTransform.sizeDelta = new Vector2(itemIconRef.rectTransform.sizeDelta.x, 70f);
                    }
                } else
                {
                    if (itemIconRef.rectTransform.localRotation.eulerAngles.z == 90f)
                    {
                        itemIconRef.rectTransform.Rotate(0, 0, -90f);
                        itemIconRef.rectTransform.sizeDelta = new Vector2(itemIconRef.rectTransform.sizeDelta.x, 32.95f);
                    }
                }
            }
            else
            {
                itemIconRef.color = new Color(255f, 255f, 255f, 0f);
                itemIconRef.sprite = null;
                //Debug.Log("item unset!");
            }
        }
        else
        {
            healthIconRef.color = new Color(255f, 255f, 255f, 0f);
            healthIconRef.sprite = null;
            itemIconRef.color = new Color(255f, 255f, 255f, 0f);
            itemIconRef.sprite = null;
            uiWidgetRef.GetComponent<Image>().sprite = DeathTag;
        }

        //Debug.Log("UI updated!");
    }

    void slide()
    {
        isSliding = true;
        regularColl.enabled = false;
        slideColl.enabled = true;

        addSlideForce();
    }

    void addSlideForce()
    {
        StartCoroutine("SlideMotion");
    }

    IEnumerator SlideMotion()
    {
        float i = slideLength;
        int direction = (flippedRight) ? 1 : -1;

        while (slideSpeed > i)
        {
            rb2d.velocity = new Vector2(direction * slideSpeed / i, rb2d.velocity.y);
            i = i + Time.deltaTime;

            if (rb2d.velocity.x < 10f && rb2d.velocity.x > -10f)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        rb2d.velocity = Vector2.zero;
        regularColl.enabled = true;
        slideColl.enabled = false;
        isSliding = false;
        yield return null;
    }

    public void startKnockback(Vector3 center, int strength)
    {
        StartCoroutine(knockBack(center, strength));
    }

    private bool ineffect;

    IEnumerator knockBack(Vector3 center, int strength)
    {
        if (!ineffect)
        {
            ineffect = true;
            Debug.Log(center + ";" +transform.position);
            Vector3 direction = new Vector2(  transform.position.x-center.x,
                 transform.position.y-center.y);
            
            direction = direction.normalized * strength;
            Debug.Log("Center :" + center);
            //direction.x *= -1;
            Vector3 end = transform.position + direction;
            Debug.Log("Direction :"+direction.x);
            
            while (transform.position != end)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, end, 10 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            ineffect = false;
        }
    }

    public void startStun(int strength)
    {
        //disable collision
        //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

        //stop character from falling and calculating collisions
        //gameObject.GetComponent<Rigidbody2D>().simulated = false;
        StartCoroutine(stun(strength));
    }

    private bool ineffectStun = false;

    IEnumerator stun(int strength)
    {
        if (!ineffectStun)
        {
                Debug.Log("Started Stun");
            ineffectStun = true;
            float num = maxSpeed;
            float jump = jumpTakeOffSpeed;


            maxSpeed = 0.00001f;
            jumpTakeOffSpeed = 0.00001f;

            yield return new WaitForSeconds(strength);
            Debug.Log("Stun: Get values back");
            maxSpeed = num;
            jumpTakeOffSpeed = jump;
            Debug.Log("End Stun");
            ineffectStun = false;
        }
    }

    IEnumerator looseLife()
    {
        lives = lives - 1;

        //remove character from camera list
        mainCameraScript.RemoveCharacter(gameObject.transform);

        //move character out of sight
        transform.position = new Vector3(-40, 0, 0);

        //stop simulating rigidbody, thus colisions
        gameObject.GetComponent<Rigidbody2D>().simulated = false;

        updateUI();

        //Debug.Log("player inactive");
        if (lives > 0)
        {
            //waits with coroutine in scaled time ie. pausing will pause this process. If you don't want this you can use WaitForSecondsRealtime
            yield return new WaitForSeconds(3);

            //continue simulating rigidbody
            gameObject.GetComponent<Rigidbody2D>().simulated = true;

            //move to camera
            transform.position = mainCamera.transform.position;

            //add character to camera list
            mainCameraScript.AddCharacter(gameObject.transform);
        }
        else
        {
            //"removes" the player from the game
            gameModeReference.removePlayerInstance(gameObject);

            gameModeReference.checkGameState();

            //Debug.Log("checked state");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Death")
        {
            //start player death coroutine
            StartCoroutine(looseLife());
        }
    }


}