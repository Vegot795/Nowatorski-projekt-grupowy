using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public float moveSpeed = 2f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;
    public SwordAttack swordAttack;
    public int Damage = 10;
    bool canMove = true;
    Vector2 movementInput;
    public Direction facingDirection;

    // Components
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Animator animator;
    CapsuleCollider2D damageCol;
    CharacterBasics CB;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    SwordAttack sword;
    UI_Controller uiController;
    PlantationScript _plantation;
    InteractionDetector interactionDetector;
    public InventoryManager inventoryManager;
    void Start()
    {
        _plantation = Object.FindFirstObjectByType<PlantationScript>();
        uiController = Object.FindFirstObjectByType<UI_Controller>();
        if (inventoryManager == null)
        {
            inventoryManager = Object.FindFirstObjectByType<InventoryManager>();
        }
        CB = GetComponent<CharacterBasics>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactionDetector = GetComponentInChildren<InteractionDetector>();

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
        if (movementInput.x < 0)
        {
            spriteRenderer.flipX = true;
            swordAttack.attackDirection = SwordAttack.AttackDirection.Left;
            facingDirection = Direction.Left;
        }
        else if (movementInput.x > 0)
        {
            spriteRenderer.flipX = false;
            swordAttack.attackDirection = SwordAttack.AttackDirection.Right;
            facingDirection = Direction.Right;
        }
        if (movementInput.y > 0)
        {
            animator.SetBool("isMovingTop", true);
            animator.SetBool("isMovingBottom", false);
            swordAttack.attackDirection = SwordAttack.AttackDirection.Top;
            facingDirection = Direction.Top;
        }
        else if (movementInput.y < 0)
        {
            animator.SetBool("isMovingTop", false);
            animator.SetBool("isMovingBottom", true);
            swordAttack.attackDirection = SwordAttack.AttackDirection.Bottom;
            facingDirection = Direction.Bottom;
        }
        else
        {
            animator.SetBool("isMovingTop", false);
            animator.SetBool("isMovingBottom", false);
        }
    }
    private bool TryMove(Vector2 direction)
    {
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

    #region ------------------------ Bindings for UI / Inventory / Shop ------------------------
    public void OnFire()
    {
        print("Fire button pressed");
        SwordAttackFunc();
    }

    public void OnOpenBuildMenu()
    {
        uiController.ToggleBuildMenu();
    }

    public void OnOpenPlantMenu()
    {
        //uiController.TogglePlantMenu();
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

    public void OnOpenInventory()
    {
        inventoryManager.ToggleInventory();
    }

    public void OnMouseLeftButtonClick()
    {
        if (uiController.isBuildingEnabled)
        {
            if (_plantation.BuildingMode == "Build")
            {
                _plantation.BuildNewField();
            }

            if (_plantation.BuildingMode == "Destroy")
            {
                _plantation.RemoveField();

            }
            return;
        }
        else
        {
            switch (inventoryManager.currentHeldSlot?.ItemInSlot)
            {
                case SeedSO seed:
                    inventoryManager.PlantHeldSeeds();
                    Debug.Log($"Mouse left button plants seed");
                    break;
                case ToolSO tool:
                    tool.UseTool();
                    break;
            }
        }
    }

    public void OnToolbarMove(InputValue input)
    {
        int direction = (int)input.Get<float>();
        if (direction > 0)
        {
            inventoryManager.MoveCurrentSlot(1);
        }
        else if (direction < 0)
        {
            inventoryManager.MoveCurrentSlot(-1);
        }
    }

    public void OnCurrentSlotForward()
    {
        inventoryManager.MoveCurrentSlot(1);
    }

    public void OnCurrentSlotBack()
    {
        inventoryManager.MoveCurrentSlot(-1);
    }

    public void OnInteraction(InputValue value)
    {
        if (value.isPressed)
        {
            //interactionDetector.TryInteract();
        }
    }

    public void OnToolTip()
    {
        uiController.ToggleTooltip();
    }
    #endregion

}
