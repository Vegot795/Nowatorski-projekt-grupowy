using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBasics : MonoBehaviour
{
    public int MaxHP = 100;
    public int CurrentHP;
    public int Damage = 10;
    public int KnockBackDistance = 2;
    public float RegenTime = 5f;
    public int RegenAmount = 1;
    public float RegenTickRate = 1f;
    public Vector3 SpawnPosition;
    public int moneyPenalty = 100;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float knockbackDuration = 0.2f;
    private ContactFilter2D knockbackFilter;
    private readonly List<RaycastHit2D> knockbackHits = new List<RaycastHit2D>();
    private const float knockbackCollisionOffset = 0.05f;
    private MobSpawner spawner;
    [SerializeField] private Slider hpSlider;
    private bool isPlayer;

    private float regenTimer = 0f;
    private float regenTickTimer = 0f;

    void Start()
    {
        SpawnPosition = transform.position;
        CurrentHP = MaxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        knockbackFilter = new ContactFilter2D();
        knockbackFilter.useLayerMask = true;
        knockbackFilter.layerMask = LayerMask.GetMask("Ground", "Wall");
        knockbackFilter.useTriggers = false;

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
                isKnockedBack = false;
        }

        if (isPlayer)
            HandleRegeneration();
    }

    private void HandleRegeneration()
    {
        if (regenTimer > 0f)
        {
            regenTimer -= Time.deltaTime;
            return;
        }

        if (CurrentHP >= MaxHP) return;

        regenTickTimer -= Time.deltaTime;
        if (regenTickTimer <= 0f)
        {
            CurrentHP = Mathf.Min(CurrentHP + RegenAmount, MaxHP);
            regenTickTimer = RegenTickRate;
            UpdateHealthBar();
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

        if (isPlayer)
        {
            regenTimer = RegenTime;
        }
        UpdateHealthBar();
    }

    private void Die()
    {
        if (gameObject == Resources.Load<GameObject>("FlyingEye"))
        {
            spawner.RemoveEnemyFromList(gameObject.GetComponent<FlyingEyeController>());
            Destroy(gameObject);
        }
        else if (isPlayer)
        {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.enabled = false;
            StartCoroutine(ShowDeadScreenSequence(2f));
            gameObject.transform.position = SpawnPosition;
            sr.enabled = true;
            gameObject.GetComponent<Money>().currentMoney -= moneyPenalty;
        }
    }

    private IEnumerator ShowDeadScreenSequence(float time)
    {
        gameObject.GetComponent<UI_Controller>().ToggleDeadscreen(true);

        yield return new WaitForSeconds(time);
        gameObject.GetComponent<UI_Controller>().ToggleDeadscreen(false);

    }

    private void KnockBack(Vector2 direction, int distance)
    {
        if (rb != null)
        {
            Vector2 normalizedDirection = direction.normalized;
            float knockbackDistance = distance;

            int count = rb.Cast(normalizedDirection, knockbackFilter, knockbackHits, knockbackDistance);
            for (int i = 0; i < count; i++)
            {
                float allowedDistance = knockbackHits[i].distance - knockbackCollisionOffset;
                if (allowedDistance < knockbackDistance)
                {
                    knockbackDistance = Mathf.Max(allowedDistance, 0f);
                }
            }

            Vector2 knockback = normalizedDirection * knockbackDistance;
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

        if (hpSlider != null)
        {
            hpSlider.value = CurrentHP;
        }
    }

    private bool StopRegenerate()
    {
        RegenTime -= Time.deltaTime;
        if(RegenTime <= 0)
        {
            RegenTime = 0;
            return true;
        }

        return false;
    }

    private void UpdateHealthBar()
    {
        if (hpSlider == null) return;
        hpSlider.value = CurrentHP;

        if (isPlayer && hpSlider.fillRect != null)
        {
            Image fillImage = hpSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float hpPerCent = (float)CurrentHP / MaxHP;
                Color max = new Color(12f / 255f, 255f / 255f, 0f / 255f, 1f); // Green #0CFF00
                Color min = new Color(255f / 255f, 0f / 255f, 20f / 255f, 1f); // Red #FF0014
                fillImage.color = Color.Lerp(min, max, hpPerCent);
            }
        }
    }
}