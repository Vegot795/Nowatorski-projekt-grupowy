using UnityEngine;

public class CharacterBasics : MonoBehaviour
{
    public int MaxHP = 100;
    public int CurrentHP;
    public int Damage = 10;
    public int KnockBackDistance = 2;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float knockbackDuration = 0.2f;
    private MobSpawner spawner;

    void Start()
    {
        CurrentHP = MaxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if(spawner == null)
        {
            spawner = FindAnyObjectByType<MobSpawner>();
        }
    }

    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }
    }

    public void TakeDamage(int damageAmount, Vector2 knockbackDirection)
    {
        CurrentHP -= damageAmount;
        if (CurrentHP <= 0)
        {
            Die();
        }

        KnockBack(knockbackDirection, KnockBackDistance);
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }

    private void Die()
    {
        if (gameObject == Resources.Load<GameObject>("FlyingEye"))
        {
            spawner.RemoveEnemyFromList(gameObject.GetComponent<FlyingEyeController>());
        }
        Destroy(gameObject);
    }

    private void KnockBack(Vector2 direction, int distance)
    {
        if (rb != null)
        {
            Vector2 knockback = direction.normalized * distance;
            rb.MovePosition(rb.position + knockback);
        }
    }

    public bool IsKnockedBack() => isKnockedBack;
}