using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemSO item;
    public int count = 1;
    public float floatSpeed;
    public float floatMaxHeight;
    public GameObject ItemIcon;
    public GameObject ItemHighLight;
    public SpriteRenderer SpriteRenderer;

    void Awake()
    {
        GetComponent<ItemPickup>().enabled = true;
        if (ItemIcon != null && item != null)
        {
            SpriteRenderer iconRenderer = ItemIcon.GetComponent<SpriteRenderer>();
            if (iconRenderer != null)
            {
                iconRenderer.sprite = item.Icon;
            }
        }
    }
    void Update()
    {
        ItemFloat();
    }

    private void ItemFloat()
    {
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatMaxHeight;
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, newY, gameObject.transform.localPosition.z);
    }

    public void Collect()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null && rb != null)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);
            float pullRadius = player.GetComponent<CircleCollider2D>().radius;
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float pullStrength = Mathf.Clamp01(1 - (distance / pullRadius));
            float pullForce = pullStrength * 10f;

            rb.linearVelocity = direction * pullForce;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemCollector"))
        {
            Collect();
        }
        else if (collision.CompareTag("PlayerHitbox"))
        {
            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.inventoryManager.addItemToInv(item, count);
                Destroy(gameObject);
            }
        }
    }

}
