using UnityEngine;
using UnityEngine.InputSystem; // WAŻNE: Dodajemy obsługę New Input System!

public class PlayerMovement : LivingEntity
{
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

    // ZMIANA: Nadpisujemy Start z klasy LivingEntity
    protected override void Start()
    {
        base.Start(); // Uruchamia kod z LivingEntity (przypisuje 100 HP i isDead = false)
        Cursor.lockState = CursorLockMode.Locked;
    }

    // ZMIANA: Własna logika śmierci gracza
    public override void Die()
    {
        Debug.Log("Gracz został zabity przez potwora!");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Zamiast niszczyć gracza, wyrzucamy go do Menu Głównego
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        // 1. ROZGLĄDANIE SIĘ (Obrót myszką) - POPRAWKA (Dodano Time.deltaTime)
        float mouseX = lookInput.x * sensitivity * Time.deltaTime;
        float mouseY = lookInput.y * sensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 2. CHODZENIE (WSAD)
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 3. PROSTA GRAWITACJA
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }

    // ==========================================
    // METODY OBSŁUGI NOWEGO INPUT SYSTEMU (Poprawione pod Events)
    // ==========================================

    // Ta metoda wywoła się automatycznie, gdy poruszysz WSAD-em
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Ta metoda wywoła się automatycznie, gdy poruszysz myszką
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    // Dokładna realizacja schematu blokowego "Logika ataku"
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Sprawdzamy tylko moment kliknięcia (naciśnięcia przycisku)
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