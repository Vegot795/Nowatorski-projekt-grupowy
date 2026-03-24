using UnityEngine;

public class AttackDamage : MonoBehaviour
{

    Collider2D swordCollider;
    Animator animator;


    private void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }
    public void AttackRight()
    {
       swordCollider.enabled = true;
       animator.SetTrigger("AttackRight");
    }

   public void AttackLeft()
    {
       swordCollider.enabled = true;
       animator.SetTrigger("AttackLeft");
    }

    public void AttackTop()
     {
       swordCollider.enabled = true;
       animator.SetTrigger("AttackTop");
    }

    public void AttackBottom()
    {
        swordCollider.enabled = true;
        animator.SetTrigger("AttackBottom");
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }
}
