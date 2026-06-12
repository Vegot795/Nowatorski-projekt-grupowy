using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Slider hpSlider;
    private bool isPlayer;

    void Start()
    {
        CurrentHP = MaxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if(spawner == null)
        {
            spawner = FindAnyObjectByType<MobSpawner>();
        }

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("GameObject is player");
            isPlayer = true;

            if(hpSlider == null)
            {
                hpSlider = GameObject.FindWithTag("HealthSlider").GetComponent<Slider>();
            }
        }
        else
        {
            hpSlider = GetComponentInChildren<Slider>();
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = MaxHP;
            hpSlider.value = CurrentHP;
        }
        else
        {
            Debug.Log($"SLIDER IS MISSING FOR {gameObject}");
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
        hpSlider.value = CurrentHP;
        if (isPlayer)
        {
            Image fillImage = hpSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float hpPerCent = (float)CurrentHP / MaxHP;
                Color max = new Color(12f / 255f, 255f / 255f, 0f / 255f, 1f);      // Green #0CFF00
                Color min = new Color(255f / 255f, 0f / 255f, 20f / 255f, 1f);      // Red #FF0014
                fillImage.color = Color.Lerp(min, max, hpPerCent);
            }
        }
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