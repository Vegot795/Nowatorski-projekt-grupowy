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
    public float pullForceMod = 10f;

    private float startTime;
    private Vector3 itemIconStartLocalPosition;



    void Awake()
    {
        startTime = Time.time;
        itemIconStartLocalPosition = ItemIcon.transform.localPosition;
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
        float timeSinceSpawn = Time.time - startTime;
        float floatY = Mathf.Sin(startTime * floatSpeed) * floatMaxHeight;
        ItemIcon.transform.localPosition = new Vector3(
            itemIconStartLocalPosition.x,
            itemIconStartLocalPosition.y + floatY,
            itemIconStartLocalPosition.z
        );
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
            float pullForce = pullStrength * pullForceMod;

            rb.linearVelocity = direction * pullForce;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.CompareTag("ItemCollector"))
        {
            Collect();
            //Debug.Log("Collecting item: " + item.name);
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
