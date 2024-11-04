using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Transforms;

public readonly partial struct LocomotionAspect : IAspect {
    public readonly RefRW<AgentBody> AgentBody;
    public readonly RefRW<AgentLocomotion> AgentLocomotion;
}