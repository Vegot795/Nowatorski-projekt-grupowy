using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
}
