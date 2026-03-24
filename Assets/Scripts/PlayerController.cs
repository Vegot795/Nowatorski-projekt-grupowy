using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    Vector2 movementInput;
    List<string> directions = new List<string>() { "Top", "Bottom", "Left", "Right" };
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D damageCol;
    CharacterBasics CB;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CB = GetComponent<CharacterBasics>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success)
            {
                success = TryMove(new Vector2(movementInput.x, 0));
            }

            if (!success)
            {
                success = TryMove(new Vector2(0, movementInput.y));
            }

            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }


        //Set vertical direction of sprites to movement direction
        if(movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }

        if(movementInput.y > 0)
        {
            animator.SetBool("isMovingTop", true);
            animator.SetBool("isMovingBottom", false);
        }
        else if (movementInput.y < 0)
        {
            animator.SetBool("isMovingTop", false);
            animator.SetBool("isMovingBottom", true);
        }
        else
        {
            animator.SetBool("isMovingTop", false);
            animator.SetBool("isMovingBottom", false);
        }
    }

    void GetDirection()
    {

    }

    private bool TryMove(Vector2 direction) {
        if(direction != Vector2.zero)
        {
            int count = rb.Cast(
                movementInput,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + movementInput * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
        
    }

    /*List<string> CheckDirection(List directions)
    {
        if (animator.GetBool("isMovingTop"))
        {
            directions = "Top"
            return Top;
        }
        else if (animator.GetBool("isMovingBottom"))
        {
            //Debug.Log("Moving down");
        }
        else if(animator.GetBool("isMoving"))
        {
            if (movementInput.x > 0)
            {
                //Debug.Log("Moving right");
            }
            else if (movementInput.x < 0)
            {
                //Debug.Log("Moving left");
            }
        }
        else
        {
            direction = null;
        }
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            
        }

    }
    public void TakeDamage(int damageAmount)
    {
        
        CB.TakeDamage(damageAmount);
    }
}
