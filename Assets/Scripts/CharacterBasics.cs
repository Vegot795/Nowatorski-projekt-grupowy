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

    void Start()
    {
        CurrentHP = MaxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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

    //save
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        CurrentHP = data.DataPlayerHP;
        Vector3 position;
        position.x = data.DataPlayerPosition[0];
        position.y = data.DataPlayerPosition[1];
        position.z = data.DataPlayerPosition[2];
        transform.position = position;
    }
}