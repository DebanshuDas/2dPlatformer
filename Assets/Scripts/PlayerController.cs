using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction shootAction;
    public GameObject pSprite;
    public float velConst;
    public float jumpConst;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        shootAction = InputSystem.actions.FindAction("Attack");
        shootAction.performed += OnShootEnd;
        shootAction.started += OnShootStart;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnShootStart(InputAction.CallbackContext context)
    {
        pSprite.GetComponent<Animator>().SetBool("isShoot", true);
        Debug.Log("Boo");
    }

    private void OnShootEnd(InputAction.CallbackContext context)
    {
        //pSprite.GetComponent<Animator>().SetBool("isShoot", false);
        Invoke("ShootEnd", 0.20f);
    }

    void ShootEnd()
    {
        pSprite.GetComponent<Animator>().SetBool("isShoot", false);
    }

    void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(moveAction.ReadValue<Vector2>().x * velConst, rb.linearVelocity.y);
        if (moveAction.ReadValue<Vector2>().y > 0&&rb.linearVelocity.y<0.01)
            rb.AddForce(new Vector2(0,jumpConst), ForceMode2D.Impulse);
        pSprite.GetComponent<Animator>().SetBool("isMotion", moveAction.ReadValue<Vector2>().x != 0);
        if (moveAction.ReadValue<Vector2>().x > 0) pSprite.GetComponent<SpriteRenderer>().flipX = false;
        else if (moveAction.ReadValue<Vector2>().x < 0) pSprite.GetComponent<SpriteRenderer>().flipX = true;
    }
}
