using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    public int Damage = 10;
    bool canMove = true;
    Vector2 movementInput;
    List<string> directions = new List<string>() { "Top", "Bottom", "Left", "Right" };
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D damageCol;
    CharacterBasics CB;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    SwordAttack sword;
    UI_Controller uiController;
    PlantationScript _plantation;
    void Start()
    {
        _plantation = Object.FindFirstObjectByType<PlantationScript>();
        uiController = Object.FindFirstObjectByType<UI_Controller>();
        CB = GetComponent<CharacterBasics>();
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
        animator.ResetTrigger("onAttack");
        swordAttack.StopAttack();
    }
    public void OnFire()
    {
        print("Fire button pressed");
        SwordAttackFunc();
    }

    public void OnOpenBuildMenu()
    {
        uiController.ToggleBuildMenu();
    }

    public void OnChangeBuildMode()
    {
        if (uiController.isBuildingEnabled)
        {
            if (_plantation.BuildingMode == "Build")
            {
                _plantation.BuildingMode = _plantation.BuildingModeList[1];
            }
            else
            {
                _plantation.BuildingMode = _plantation.BuildingModeList[0];
            }
        }       
    }

    public void OnMouseLeftButtonClick()
    {
        if (uiController.isBuildingEnabled)
        {
            if(_plantation.BuildingMode == "Build")
            {
                _plantation.BuildNewField();
            }

            if (_plantation.BuildingMode == "Destroy")
            {
                _plantation.RemoveField();

            }
        }
        
    }
}
