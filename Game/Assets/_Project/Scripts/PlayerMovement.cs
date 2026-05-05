using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ustawienia Ruchu")]
    public float moveSpeed = 5f;
    public float sensitivity = 2f;

    [Header("Referencje")]
    public CharacterController controller;
    public Transform cameraTransform;

    private float verticalRotation = 0f;

    void Start()
    {
        // Ukrywamy kursor myszki
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. ROZGLĄDANIE SIĘ
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Ograniczenie ruchu góra/dół (zabezpieczenie przed "fikołkami")
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Obrót kamery (góra/dół) i obrót gracza (lewo/prawo)
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 2. CHODZENIE
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        // Oblicz kierunek ruchu względem tego, gdzie patrzy kamera
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Rusz kontrolerem
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 3. PROSTA GRAWITACJA (Żeby gracz nie lewitował)
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }
}