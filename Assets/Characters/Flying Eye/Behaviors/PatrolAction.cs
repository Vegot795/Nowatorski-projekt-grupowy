using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Tilemaps;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Patrol Nearby area", category: "Action", id: "af63d86a38dd118a6ecbc39eb8449d7c")]
public partial class PatrolAction : Action
{
    [SerializeReference] public BlackboardVariable<float> Speed;
    [SerializeReference] public BlackboardVariable<float> PatrolRange;
    [SerializeReference] public BlackboardVariable<float> ARRIVAL_THRESHOLD;
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Tilemap> Ground;


    private Vector2 m_CurrentPosition;
    private Vector2 m_TargetPoint;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_isInitialized = false;

    protected override Status OnStart()
    {
        if (Agent == null || PatrolRange == null || Speed == null)
        {
            return Status.Failure;
        }
        return Initialize();
    }

    protected override Status OnUpdate()
    {
        if (Agent == null || PatrolRange == null || Speed == null)
        {
            return Status.Failure;
        }

        if(!m_isInitialized)
        {
            return Status.Failure;
        }
        

        m_CurrentPosition = Agent.Value.transform.position;
        float distance = Vector2.Distance(m_CurrentPosition, m_TargetPoint);

        if (distance <= ARRIVAL_THRESHOLD)
        {
            return Status.Success; 
        }

        Vector2 direction = (m_TargetPoint - m_CurrentPosition).normalized;
        Vector2 movement = direction * Speed.Value * Time.fixedDeltaTime;

        Rigidbody2D rb = Agent.Value.GetComponent<Rigidbody2D>();
        if (m_Rigidbody2D != null)
        {
            m_Rigidbody2D.MovePosition(m_CurrentPosition + movement);
        }
        else
        {
            Agent.Value.transform.position = m_CurrentPosition + movement;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {

    }

    private Status Initialize ()
    {
        BoundsInt bounds = Ground.Value.cellBounds;

        if (bounds.size.x == 0 || bounds.size.y == 0)
        {
            return Status.Failure;
        }

        float randomX = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        float randomY = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
        m_TargetPoint = new Vector2(randomX, randomY);

        m_Rigidbody2D = Agent.Value.GetComponent<Rigidbody2D>();

        m_isInitialized = true;


        return Status.Running;
    }
}
