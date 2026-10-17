using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float deadZone = 0.05f;     // distância horizontal mínima do mouse pra ele virar
    public float stopDistance = 0.1f;  // evita tremer ao chegar no ponteiro
    public bool alwaysFollow = false;  // true = anda sempre, false = só segurando o botão esquerdo
    public bool spriteFacesRight = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera cam;
    private Vector2 mouseWorldPos;
    private bool moving;
    private bool pressStartedOnUI;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        mouseWorldPos = cam.ScreenToWorldPoint(screenPos);

        // se o clique começou em cima de UI, ignora até soltar
        if (Mouse.current.leftButton.wasPressedThisFrame)
            pressStartedOnUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        bool holding = Mouse.current.leftButton.isPressed && !pressStartedOnUI;
        moving = alwaysFollow || holding;

        // vira pro lado do mouse
        float dx = mouseWorldPos.x - rb.position.x;
        if (Mathf.Abs(dx) > deadZone)
        {
            bool mouseOnLeft = dx < 0;
            sr.flipX = spriteFacesRight ? mouseOnLeft : !mouseOnLeft;
        }
    }

    void FixedUpdate()
    {
        Vector2 dir = mouseWorldPos - rb.position;

        if (moving && dir.magnitude > stopDistance)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, mouseWorldPos, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }
}