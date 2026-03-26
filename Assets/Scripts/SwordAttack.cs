using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    Vector2 rightAttackOffset = new Vector2(0.36f, 0);
    Vector2 topAttackOffset;
    Vector2 bottomAttackOffset;
    Vector2 swordColidPos;
    Collider2D swordCollider;

    Animator animator;
    

    public enum AttackDirection
    {
        Right,
        Left,
        Top,
        Bottom
    }

    public AttackDirection attackDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        rightAttackOffset = transform.position;
        swordCollider.enabled = false;
        animator = GetComponentInParent<Animator>();
        if(animator != null)
        {
            print("Animator found in parent");
        }
    }


    public void Attack()
    {
        switch (attackDirection)
        {
            case AttackDirection.Right:
                AttackRight();
                break;
            case AttackDirection.Left:
                AttackLeft();
                break;
            case AttackDirection.Top:
                AttackTop();
                break;
            case AttackDirection.Bottom:
                AttackBottom();
                break;
        }

    }

    private  void AttackRight()
    {
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
        animator.SetTrigger("onAttack");
    }

    private void AttackLeft() 
    {
        swordCollider.enabled = true;
        transform.localPosition = new Vector2(rightAttackOffset.x * -1, rightAttackOffset.y);
        animator.SetTrigger("onAttack");
    }

    private void AttackTop()
    {
        swordCollider.enabled = true;
        transform.localPosition = new Vector2(0.12f, 0.36f);
        animator.SetTrigger("onAttack");
    }
    private void AttackBottom()
    {
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(0.12f, -0.36f);
        animator.SetTrigger("onAttack");
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;

    }

    void OnDrawGizmos()
    {
        if (swordCollider == null)
            swordCollider = GetComponent<Collider2D>();

        if (swordCollider != null && swordCollider.enabled)
        {
            Gizmos.color = Color.red;

            if (swordCollider is BoxCollider2D box)
            {
                Vector2 pos = (Vector2)transform.position + box.offset;
                Vector3 size = new Vector3(box.size.x, box.size.y, 1f);
                Gizmos.DrawWireCube(pos, size);
            }
            else if (swordCollider is CircleCollider2D circle)
            {
                Vector2 pos = (Vector2)transform.position + circle.offset;
                Gizmos.DrawWireSphere(pos, circle.radius);
            }
            // Add more collider types if needed
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.GetComponent<GameObject>();
        if (enemy != null) ;
        
    }
}
