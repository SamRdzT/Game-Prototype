using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float airControl = 0.8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float jumpForce2 = 10f;
    [SerializeField] private int defaultExtraJumps = 1;

    [Header("Flight Settings")]
    [SerializeField] private float flightSpeed = 10f;
    [SerializeField] private float flightDuration = 5f;

    [Header("UI Elements")]
    [SerializeField] private Text flightTimerText; // Reemplazamos el Slider por Texto
    [SerializeField] private Text jumpCountText;

    [Header("Other")]
    [SerializeField]
    private string endScreenName = "EndScreen";

    private int extraJump;
    private int currentMaxExtraJumps;
    private bool isGrounded;
    private Rigidbody2D body;

    private bool isFlying = false;
    private float flightTimer;
    private float originalGravity;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        originalGravity = body.gravityScale;
        currentMaxExtraJumps = defaultExtraJumps;
        extraJump = currentMaxExtraJumps;

        // Inicializar UI: ocultar el texto de vuelo al empezar
        if (flightTimerText != null)
        {
            flightTimerText.gameObject.SetActive(false);
        }
        UpdateJumpUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.G))
            SceneManager.LoadScene(endScreenName);

        if (isFlying)
        {
            UpdateFlight();
        }
        else
        {
            HandleNormalMovement();
        }
    }

    private void HandleNormalMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float currentSpeed = isGrounded ? speed : speed * airControl;
            body.velocity = new Vector2(horizontalInput * currentSpeed, body.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void UpdateFlight()
    {
        flightTimer -= Time.deltaTime;

        // Actualizar el texto con números enteros (5, 4, 3, 2, 1, 0)
        if (flightTimerText != null)
        {
            // Mathf.Max asegura que no muestre números negativos
            int secondsLeft = Mathf.CeilToInt(Mathf.Max(0, flightTimer));
            flightTimerText.text = "VUELO: " + secondsLeft + "s";

            // Efecto visual: si queda poco tiempo (menos de 2s), ponerlo en rojo
            flightTimerText.color = (flightTimer < 2f) ? Color.red : Color.magenta;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        body.velocity = new Vector2(h * flightSpeed, v * flightSpeed);

        if (flightTimer <= 0)
        {
            StopFlying();
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
            UpdateJumpUI();
        }
        else if (extraJump > 0)
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce2);
            extraJump--;
            UpdateJumpUI();
        }
    }

    private void UpdateJumpUI()
    {
        if (jumpCountText != null)
        {
            int totalAvailable = (isGrounded ? 1 : 0) + extraJump;
            jumpCountText.text = "SALTOS: " + totalAvailable;
            jumpCountText.color = (currentMaxExtraJumps > 1) ? Color.green : Color.white;
        }
    }

    private void StartFlying()
    {
        if (isFlying) return;
        isFlying = true;
        flightTimer = flightDuration;
        body.gravityScale = 0;

        if (flightTimerText != null)
        {
            flightTimerText.gameObject.SetActive(true);
        }
    }

    private void StopFlying()
    {
        isFlying = false;
        body.gravityScale = originalGravity;
        if (flightTimerText != null)
        {
            flightTimerText.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            currentMaxExtraJumps = defaultExtraJumps;
            extraJump = currentMaxExtraJumps;
            UpdateJumpUI();
        }
        else if (collision.gameObject.CompareTag("UnlimitedJumps"))
        {
            isGrounded = true;
            currentMaxExtraJumps = 4;
            extraJump = currentMaxExtraJumps;
            UpdateJumpUI();
        }
        else if (collision.gameObject.CompareTag("FlightPlatform"))
        {
            StartFlying();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UnlimitedJumps"))
        {
            isGrounded = false;
            UpdateJumpUI();
        }
    }
}