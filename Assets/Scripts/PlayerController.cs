using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;

    Vector2 movementInput;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    bool canMove = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UnlockMovement();
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
            swordAttack.attackDirection = SwordAttack.AttackDirection.Left;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
            swordAttack.attackDirection = SwordAttack.AttackDirection.Right;
        }

        if(movementInput.y > 0)
        {
            animator.SetBool("isMovingTop", true);
            animator.SetBool("isMovingBottom", false);
            swordAttack.attackDirection = SwordAttack.AttackDirection.Top;
        }
        else if (movementInput.y < 0)
        {
            animator.SetBool("isMovingTop", false);
            animator.SetBool("isMovingBottom", true);
            swordAttack.attackDirection = SwordAttack.AttackDirection.Bottom;
        }
        else
        {
            animator.SetBool("isMovingTop", false);
            animator.SetBool("isMovingBottom", false);
        }
    }

    private bool TryMove(Vector2 direction) {
        if (canMove)
        {    
            if (direction != Vector2.zero)
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
        else
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }


    public void SwordAttackFunc()
    {
        LockMovement();
        
        swordAttack.Attack();
        print("Attacking in direction: " + swordAttack.attackDirection);
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    public void OnFire()
    {
        print("Fire button pressed");
        SwordAttackFunc();
    }
}
