using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;


    [Header("Jumping")]
    [SerializeField] float jumpForce;
    public LayerMask groundLayer;
    public GameObject groundCheck;
    public float coyoteTime = 0.4f;
    private float coyoteTimeCounter;

    [Header("Knockback")]
    [SerializeField] float knockbackAmount;
    public float knockbackTime = 0.2f;
    public float hitDirectionForce = 10f;
    public float constForce = 0;
    public float inputForce = 0;
    public AnimationCurve knockbackForceCurve;

    [Header("Mining")]
    [SerializeField] public GameObject miningHitBox;
    [SerializeField] public float miningCooldown;

    [Header("SFX")]
    public AudioClip minesfx;
    public AudioClip breaksfx;
    AudioSource SFXSource;

    public bool IsBeingKnockedBack { get; private set; }
    private Coroutine knockbackCoroutine;
    bool isGrounded;

    Rigidbody2D rb;
    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool canMine;
    [HideInInspector] public bool touchingTile;
    [HideInInspector] public bool miningButtonPressed;
    [HideInInspector] public float targetTime;
    [HideInInspector] public float movementDir;
    [HideInInspector] public Vector2 miningDir;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SFXSource = GetComponent<AudioSource>();
        targetTime = miningCooldown;
    }

    void Update()
    {
        // Input Manager
        InputManager();

        // Mining Direction
        MiningDirectionManager();

        // Mining Cooldown
        MiningCooldown();

        // Jumping
        Jump();
    }

    void FixedUpdate()
    {
        // Moving
        MovementManager();

    }

    void InputManager()
    {
        // Movement Direction
        movementDir = Input.GetAxisRaw("Horizontal");

        // Mining
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.X))
        {
            miningButtonPressed = true;
        }
        else
        {
            miningButtonPressed = false;
        }
    }

    void MiningCooldown()
    {
        if (touchingTile)
            targetTime -= Time.deltaTime;
        else
            targetTime = miningCooldown;

        if (targetTime <= 0.0f)
        {
            canMine = true;
            targetTime = miningCooldown;
        }
    }

    void MiningDirectionManager()
    {
        miningDir.x = Input.GetAxisRaw("Horizontal");
        miningDir.y = Input.GetAxisRaw("Vertical");

        // X Axis
        if (miningDir.x == 1) // x = 1
            miningHitBox.transform.position = Vector2.MoveTowards(transform.position, new Vector2(this.transform.position.x + .4f, this.transform.position.y), 1);
        if (miningDir.x == -1) // x = -1
            miningHitBox.transform.position = Vector2.MoveTowards(transform.position, new Vector2(this.transform.position.x - .4f, this.transform.position.y), 1);

        // Y Axis
        if (miningDir.y == 1) // x = 1
            miningHitBox.transform.position = Vector2.MoveTowards(transform.position, new Vector2(this.transform.position.x, this.transform.position.y + .65f), 1);
        if (miningDir.y == -1) // x = -1
            miningHitBox.transform.position = Vector2.MoveTowards(transform.position, new Vector2(this.transform.position.x, this.transform.position.y - .65f), 1);

        // 0-0
        if (miningDir.y == 0 && miningDir.x == 0) // No input
            miningHitBox.transform.position = Vector2.MoveTowards(transform.position, new Vector2(this.transform.position.x, this.transform.position.y), 1); isMoving = false;

        // Is Moving?
        if (miningDir.y != 0 || miningDir.x != 0) // Input Returns Into A Bool
            isMoving = true;
    }

    void MovementManager()
    {
        if (!IsBeingKnockedBack)
        {
            rb.linearVelocity = new Vector2(movementDir * speed, rb.linearVelocityY);
        }
    }

    void Jump()
    {
        isGrounded = Physics2D.OverlapCapsule(groundCheck.transform.position, new Vector2(0.2f, 0.2f), CapsuleDirection2D.Horizontal, 0, groundLayer);

        // Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump Force
        if (!IsBeingKnockedBack && Input.GetButtonDown("Jump") && coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);

            coyoteTimeCounter = 0f;
        }
    }

    public IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection)
    {
        IsBeingKnockedBack = true;

        Vector2 _hitForce;
        Vector2 _constantForce;
        Vector2 _knockbackForce;
        Vector2 _combinedForce;
        float _time = 0f;

        _constantForce = constantForceDirection * constForce;

        float _elapsedTime = 0f;
        while (_elapsedTime < knockbackTime)
        {
            //Iterate the timer
            _elapsedTime += Time.fixedDeltaTime;
            _time += Time.fixedDeltaTime;

            _hitForce = hitDirection * hitDirectionForce * knockbackForceCurve.Evaluate(_time);

            _knockbackForce = _hitForce + _constantForce;

            if (movementDir != 0)
            {
                _combinedForce = _knockbackForce + new Vector2(movementDir, 0f);
            }
            else
            {
                _combinedForce = _knockbackForce;
            }

            rb.linearVelocity = _combinedForce;

            yield return new WaitForFixedUpdate();
        }

        IsBeingKnockedBack = false;
    }

    public void CallKnockback(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection)
    {
        knockbackCoroutine = StartCoroutine(KnockbackAction(hitDirection, constantForceDirection, inputDirection));
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
