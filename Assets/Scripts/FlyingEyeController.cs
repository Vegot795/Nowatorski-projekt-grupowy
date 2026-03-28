using Unity.VisualScripting;
using UnityEngine;

public class FlyingEyeController : MonoBehaviour
{
    public int Damage = 10;

    public float moveSpeed = 1f;
    public float visionRange = 5f;

    private bool wasDamaged = false;

    PlayerController Player;
    PlayerController ClosestPlayer;

    Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        col = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!wasDamaged)
        {
            if (!collision.CompareTag("PlayerHitbox")) return;

            var player = collision.GetComponentInParent<CharacterBasics>();
            player.TakeDamage(Damage, (collision.transform.position - transform.position).normalized);
            wasDamaged = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!wasDamaged)
        {
            if (!collision.CompareTag("PlayerHitbox")) return;

            var player = collision.GetComponentInParent<CharacterBasics>();
            player.TakeDamage(Damage, (collision.transform.position - transform.position).normalized);
            wasDamaged = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitbox"))
        {
            wasDamaged = false;
        }
    }
}