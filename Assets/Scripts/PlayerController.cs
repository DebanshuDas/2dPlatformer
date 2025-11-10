using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction sprintAction;
    public GameObject pSprite;
    public GameObject arrow;
    public float velConst;
    public float dashVelConst;
    public float jumpConst;

    private bool isDashCurr=false;
    private bool isDashLegal=true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        shootAction = InputSystem.actions.FindAction("Attack");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        sprintAction.performed += OnDash;
        shootAction.performed += OnShootEnd;
        shootAction.started += OnShootStart;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (!isDashLegal) return;
        Vector2 dashVel = new Vector2(dashVelConst, 0);
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = (pSprite.GetComponent<SpriteRenderer>().flipX ? -1 : 1) * dashVel;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionY|RigidbodyConstraints2D.FreezeRotation;
        isDashCurr = true;pSprite.GetComponent<Animator>().SetBool("isDash", true);
        isDashLegal = false;
        Invoke("endDash", 2.0f);
        Invoke("endCoolDown", 10.0f);
    }

    void endDash() {
        isDashCurr = false;pSprite.GetComponent<Animator>().SetBool("isDash", false);
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
    }
    void endCoolDown() { isDashLegal = true; }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnShootStart(InputAction.CallbackContext context)
    {
        pSprite.GetComponent<Animator>().SetBool("isShoot", true);
       
    }

    private void OnShootEnd(InputAction.CallbackContext context)
    {
        //pSprite.GetComponent<Animator>().SetBool("isShoot", false);
        Invoke("ShootEnd", 0.20f);
        
    }

    void ShootEnd()
    {
        pSprite.GetComponent<Animator>().SetBool("isShoot", false);
        Vector2 pos = new Vector2(transform.position.x + 1.0f, transform.position.y);
         GameObject currArrow = Instantiate(arrow, pos, transform.rotation);
        float rotCurr = 0.0f;float shootCurr = 0.5f;
        Vector2 dir = new Vector2((float)Math.Cos(rotCurr * Math.PI / 180), (float)Math.Sin(rotCurr * Math.PI / 180));
        currArrow.GetComponent<Rigidbody2D>().AddForce(dir * shootCurr * 5.0f, ForceMode2D.Impulse);
    }

    void FixedUpdate()
    {
        if (isDashCurr) return;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(moveAction.ReadValue<Vector2>().x * velConst, rb.linearVelocity.y);
        if (moveAction.ReadValue<Vector2>().y > 0&&rb.linearVelocity.y<0.01)
            rb.AddForce(new Vector2(0,jumpConst), ForceMode2D.Impulse);
        pSprite.GetComponent<Animator>().SetBool("isMotion", moveAction.ReadValue<Vector2>().x != 0);
        if (moveAction.ReadValue<Vector2>().x > 0) pSprite.GetComponent<SpriteRenderer>().flipX = false;
        else if (moveAction.ReadValue<Vector2>().x < 0) pSprite.GetComponent<SpriteRenderer>().flipX = true;
    }
}
