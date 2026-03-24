using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check If target is in attack range", story: "Unit validates if the target (player) is in text", category: "Action", id: "bda0a34ef02123331ea5c5b2a2ec3120")]
public partial class CheckIfTargetIsInAttackRangeAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> VisionRange;

    protected override Status OnStart()
    {
        if(Player?.Value == null || Self?.Value == null || VisionRange?.Value == null)
            return Status.Failure;


        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        float distanceToPlayer = Vector2.Distance(Self.Value.transform.position, Player.Value.transform.position);

        if (distanceToPlayer <= VisionRange)
        {
            return Status.Success;
        }

        return Status.Failure;
    }

    protected override void OnEnd()
    {


    }
}

