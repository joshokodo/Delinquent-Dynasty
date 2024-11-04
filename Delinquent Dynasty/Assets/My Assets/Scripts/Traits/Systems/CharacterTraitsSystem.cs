using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(PostActionsGroup), OrderLast = true)]
[BurstCompile]
public partial struct CharacterTraitsSystem : ISystem {
    private ActionDataStore _dataStore;

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<InGameTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state){
        var time = SystemAPI.GetSingleton<InGameTime>();
        var stateChangeSpawn = SystemAPI.GetSingletonBuffer<CharacterStateChangeSpawnElement>();

        var jh = new CheckCharacterTraitsJob(){
            InGameTime = time,
            StateChangeSpawn = stateChangeSpawn,
        }.Schedule(state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(jh, state.Dependency);
    }
}

[BurstCompile]
public partial struct CheckCharacterTraitsJob : IJobEntity {
    public InGameTime InGameTime;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;


    public void Execute(Entity e, DynamicBuffer<TraitElement> traits){
        for (var i = 0; i < traits.Length;){
            var traitElement = traits[i];
            if (traitElement.StartTime > 0 && InGameTime.TotalInGameSeconds >= traitElement.ExpirationTime){
                traits.RemoveAt(i);
                StateChangeSpawn.Add(new CharacterStateChangeSpawnElement(){
                    Character = e,
                    TraitsChanged = true
                });
            }
            else{
                i++;
            }
        }
    }
}