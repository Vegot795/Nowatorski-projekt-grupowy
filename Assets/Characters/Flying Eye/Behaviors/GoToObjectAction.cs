using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Go to Object", story: "As [Self] go to [Player] location", category: "Action", id: "d03e134db1ed18e8ae3dbd9c31b4ae69")]
public partial class GoToObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(2.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.3f);
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.5f);

    private Rigidbody2D m_Rigidbody2D;
    private Vector2 m_LastPlayerPosition;
    private float m_ColliderOffset;
    private float m_CurrentSpeed;

    protected override Status OnStart()
    {
        if (Self.Value == null || Player.Value == null)
        {
            return Status.Failure;
        }

        return Initialize();
            }

    protected override Status OnUpdate()
    {
        if (Self.Value == null || Player.Value == null)
        {
            return Status.Failure;
        }

        // Get current player position
        Vector2 currentPlayerPosition = Player.Value.transform.position;

        // Update target if player has moved
        bool playerMoved = !Mathf.Approximately(m_LastPlayerPosition.x, currentPlayerPosition.x)
            || !Mathf.Approximately(m_LastPlayerPosition.y, currentPlayerPosition.y);

        if (playerMoved)
        {
            m_LastPlayerPosition = currentPlayerPosition;
        }

        // Calculate distance to player
        float distance = GetDistanceToPlayer();
        bool destinationReached = distance <= (DistanceThreshold.Value + m_ColliderOffset);

        if (destinationReached)
        {
            return Status.Success;
        }
        else
        {
            // Move toward player in 2D space
            m_CurrentSpeed = MoveTowardPlayer(distance);
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Clean up if needed
    }

    private Status Initialize()
    {
        m_LastPlayerPosition = Player.Value.transform.position;

        // Calculate collider offset for stopping distance
        m_ColliderOffset = 0.0f;
        Collider2D selfCollider = Self.Value.GetComponentInChildren<Collider2D>();
        if (selfCollider != null)
        {
            Vector2 colliderSize = selfCollider.bounds.size;
            m_ColliderOffset += Mathf.Max(colliderSize.x, colliderSize.y) * 0.5f;
        }

        Collider2D playerCollider = Player.Value.GetComponentInChildren<Collider2D>();
        if (playerCollider != null)
        {
            Vector2 colliderSize = playerCollider.bounds.size;
            m_ColliderOffset += Mathf.Max(colliderSize.x, colliderSize.y) * 0.5f;
        }

        // Check if already at destination
        if (GetDistanceToPlayer() <= (DistanceThreshold.Value + m_ColliderOffset))
        {
            return Status.Success;
        }

        // Get Rigidbody2D component for physics-based movement
        m_Rigidbody2D = Self.Value.GetComponent<Rigidbody2D>();

        return Status.Running;
    }

    private float GetDistanceToPlayer()
    {
        Vector2 selfPosition = Self.Value.transform.position;
        Vector2 playerPosition = Player.Value.transform.position;
        return Vector2.Distance(selfPosition, playerPosition);
    }

    private float MoveTowardPlayer(float distance)
    {
        Vector2 selfPosition = Self.Value.transform.position;
        Vector2 playerPosition = Player.Value.transform.position;

        // Calculate direction toward player (NOT away from player)
        Vector2 direction = (playerPosition - selfPosition).normalized;

        // Calculate speed with slowdown near target
        float currentSpeed = Speed.Value;
        if (distance < SlowDownDistance.Value)
        {
            currentSpeed = Speed.Value * (distance / SlowDownDistance.Value);
            currentSpeed = Mathf.Max(currentSpeed, Speed.Value * 0.1f); // Minimum speed
        }

        // Apply movement
        Vector2 movement = direction * currentSpeed * Time.fixedDeltaTime;

        if (m_Rigidbody2D != null)
        {
            // Physics-based movement
            m_Rigidbody2D.MovePosition(selfPosition + movement);
        }
        else
        {
            // Transform-based movement
            Self.Value.transform.position = selfPosition + movement;
        }

        return currentSpeed;
    }
}

