using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float accelerationSpeed;
    public float jumpSpeed;
    public float slideTime;
    public float slideCheckDistance;
    public float turnSpeed;
    public float maxMoveSpeed;

    private float animationSpeed;

    public PhysicMaterial playerFriction;
    public Animator animator;
    public Slider speedIndicator;
    public RawImage speedIndicatorFill;
    public Gradient speedMeterGradient;
    public Transform clouds;

    public CanvasGroup endScreen;
    public Text reason;
    public Text finalScore;

    private Rigidbody rb;
    private CapsuleCollider cc;

    private float distToGround;
    private float airTime;
    [SerializeField]
    private bool grounded;
    private bool sliding;
    private bool outOfEnergy;
    private bool fallOutWorld;
    private bool isWinning;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        distToGround = cc.bounds.extents.y;

        //reset
        grounded = true;
        playerFriction.staticFriction = 1f;
        playerFriction.dynamicFriction = 1f;
    }

    private void FixedUpdate()
    {
        if (grounded && rb.velocity.x < maxMoveSpeed)
        {
            if (sliding) { rb.AddForce(transform.right * (accelerationSpeed / 1.5f)); return; }
            rb.AddForce(transform.right * accelerationSpeed);
        }

        var test = rb.velocity.x / maxMoveSpeed;
        animationSpeed = Mathf.Clamp(test, 0, 1);
        if (animationSpeed <= 0.1f && Time.timeSinceLevelLoad > 5 && outOfEnergy == false && fallOutWorld == false && isWinning == false)
        {
            StartCoroutine(ShowEndScreen(false));
        }
        speedIndicator.value = animationSpeed;
        speedIndicatorFill.color = speedMeterGradient.Evaluate(animationSpeed);
        animator.SetFloat("Speed", animationSpeed);

        if (transform.position.y < -3.5f && fallOutWorld == false && isWinning == false)
        {
            StartCoroutine(ShowEndScreen(true));
        }
    }

    void Update()
    {
        Debug.DrawRay(transform.position + (Vector3.right * slideCheckDistance) * rb.velocity.x, Vector3.up, Color.red);

        var turn = Input.GetAxis("Horizontal");
        if (turn != 0)
        {
            transform.localRotation = Quaternion.Euler(0, transform.rotation.y + turn * turnSpeed, 0);
        }

        if (Physics.Raycast(transform.position, -Vector3.up, distToGround + .1f) == false && grounded)
        {
            StartCoroutine(Falling());
        }

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            StartCoroutine(Jumping());
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || (Input.GetKeyDown(KeyCode.C))) && !sliding)
        {
            StartCoroutine(Sliding());
        }
    }

    private IEnumerator Jumping()
    {
        airTime = 0;
        animator.SetBool("Jump", true);

        PlayerAirborne(true);

        cc.height = 2;
        cc.center = Vector3.zero;

        yield return new WaitForSeconds(.25f);

        while (Physics.Raycast(transform.position, -Vector3.up, distToGround + .1f) == false)
        {
            airTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("Jump", false);

        if (airTime > 1.35f)
        {
            ChaosController.Instance.SetChaosText("Nice Airtime", 25);
        }
        Debug.Log(airTime);

        yield return new WaitForSeconds(.2f);

        PlayerAirborne(false);

        yield return null;
    }

    private IEnumerator Falling()
    {
        airTime = 0;
        animator.SetBool("Falling", true);

        PlayerAirborne(true);

        yield return new WaitForSeconds(.25f);

        while (Physics.Raycast(transform.position, -Vector3.up, distToGround + .1f) == false)
        {
            airTime += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("Falling", false);

        yield return new WaitForSeconds(.15f);

        PlayerAirborne(false);

        yield return null;
    }

    private IEnumerator Sliding()
    {
        sliding = true;

        while (Physics.Raycast(transform.position, -Vector3.up, distToGround + .1f) == false)
        {
            yield return null;
        }

        animator.SetBool("Slide", true);

        cc.height = 1;
        cc.center = new Vector3(0, -.5f, 0);

        //playerFriction.staticFriction = 1f;
        //playerFriction.dynamicFriction = 1f;

        yield return new WaitForSeconds(slideTime);

        while (Physics.Raycast(transform.position + (Vector3.right * slideCheckDistance) * rb.velocity.magnitude, Vector3.up, distToGround + .5f))
        {
            yield return null;
        }

        animator.SetBool("Slide", false);


        yield return new WaitForSeconds(.2f);

        cc.height = 2;
        cc.center = Vector3.zero;
        sliding = false;

        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "lightObstacle")
        {
            collision.rigidbody.AddExplosionForce(10, transform.position, 4);
        }

        if (collision.transform.tag == "BigObstacle")
        {
            collision.rigidbody.AddExplosionForce(15, transform.position, 5);
            if (collision.transform.GetComponent<ChaosArea>())
            {
                var tmp = collision.transform.GetComponent<ChaosArea>();
                ChaosController.Instance.SetChaosText(tmp);
                Destroy(tmp);
            } 
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "helpBorder")
        {
            HelpText.Instance.ShowNext();
        }

        if (other.tag == "endTrigger")
        {
            StartCoroutine(ShowEndScreen(false, true));
        }
    }

    private IEnumerator ShowEndScreen(bool fallRestart, bool winning = false)
    {
        if (winning)
        {
            isWinning = true;
            StartCoroutine(FadeCanvasToAlpha(.25f));

            reason.text = "YOu reached your escape vehicle";
            finalScore.text = ChaosController.Instance.totalScoreText.text;

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    yield return null;
                }
                yield return null;
            }

        }
        if (fallRestart)
        {
            fallOutWorld = true;
            AudioController.Instance.PlayFallSound();
            clouds.parent = transform;
            reason.text = "You fell of the building";
        } else
        {
            outOfEnergy = true;
            AudioController.Instance.PlayOutOfEnergySound();
            accelerationSpeed = 0.5f;
            reason.text = "Your speed got to low";
        }

        finalScore.text = ChaosController.Instance.totalScoreText.text;

        yield return new WaitForSeconds(.5f);

        StartCoroutine(FadeCanvasToAlpha(.75f));

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                yield return null;
            }
            yield return null;
        }
    }

    public IEnumerator FadeCanvasToAlpha(float t)
    {
        while (endScreen.alpha < 1.0f)
        {
            endScreen.alpha += Time.deltaTime / t;
            yield return null;
        }
    }

    private void PlayerAirborne(bool air)
    {
        if (air)
        {
            grounded = false;

            playerFriction.staticFriction = 0f;
            playerFriction.dynamicFriction = 0f;
        } else
        {
            grounded = true;

            playerFriction.staticFriction = 1f;
            playerFriction.dynamicFriction = 1f;
        }
    }
}

