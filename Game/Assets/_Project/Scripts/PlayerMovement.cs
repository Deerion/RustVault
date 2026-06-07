using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // NOWE: Biblioteka UI

public class PlayerMovement : LivingEntity
{
    [Header("Interfejs")]
    public Slider healthSlider; // NOWE: Referencja do paska zdrowia

    [Header("Ustawienia Ruchu")]
    public float moveSpeed = 5f;
    public float sensitivity = 50f;

    [Header("Ustawienia Walki")]
    public float attackRange = 3.5f;
    public int attackDamage = 10;

    [Header("Referencje")]
    public CharacterController controller;
    public Transform cameraTransform;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation = 0f;

    protected override void Start()
    {
        base.Start();
        Cursor.lockState = CursorLockMode.Locked;

        // NOWE: Ustawienie suwaka na starcie
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // NOWE: Nadpisujemy otrzymywanie obrażeń, żeby zaktualizować suwak
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount); // Najpierw wykonuje kod z LivingEntity (odejmuje HP)

        // Potem aktualizuje pasek na ekranie
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    public override void Die()
    {
        Debug.Log("Gracz został zabity przez potwora!");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Gracz klika przycisk ataku.");

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, attackRange))
            {
                Debug.Log($"Promień wykrył obiekt: {hit.collider.name}");

                IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();

                if (damageableObject != null)
                {
                    damageableObject.TakeDamage(attackDamage);
                }
                else
                {
                    Debug.Log("Obiekt niezniszczalny / brak interfejsu IDamageable.");
                }
            }
            else
            {
                Debug.Log("Promień w nic nie trafił (brak obiektu w zasięgu).");
            }
        }
    }
}