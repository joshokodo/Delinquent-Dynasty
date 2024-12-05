using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct TargetsGroup {
    public FixedList512Bytes<TargetsGroupData> Targets;
    public Entity OriginSelf => GetTargetEntity(TargetType.SELF);

    public void ClearTargets(){
        Targets.Clear();
    }

    public void SetTargets(ActionElement action, DynamicBuffer<TargetElement> targets){
        Targets.Clear();
        foreach (var targetElement in targets){
            if (targetElement.ParentId == action.ActionId && (targetElement.Data.TargetEntity != Entity.Null ||
                                                              !targetElement.Data.EnumValue.IsBlank())){
                Targets.Add(new TargetsGroupData(){
                    Entity = targetElement.Data.TargetEntity,
                    TargetType = targetElement.Data.TargetType,
                    EnumValue = targetElement.Data.EnumValue
                });
            }
        }
    }

    public void SetTargets(FixedList4096Bytes<ActiveActionTargetElement> targets){
        Targets.Clear();
        foreach (var targetElement in targets){
            if (targetElement.Data.TargetEntity != Entity.Null || !targetElement.Data.EnumValue.IsBlank()){
                Targets.Add(new TargetsGroupData(){
                    Entity = targetElement.Data.TargetEntity,
                    TargetType = targetElement.Data.TargetType,
                    EnumValue = targetElement.Data.EnumValue,
                });
            }
        }
    }

    public void SetTargets(DynamicBuffer<TargetElement> targets){
        Targets.Clear();
        foreach (var targetElement in targets){
            if (targetElement.Data.TargetEntity != Entity.Null || !targetElement.Data.EnumValue.IsBlank()){
                Targets.Add(new TargetsGroupData(){
                    Entity = targetElement.Data.TargetEntity,
                    TargetType = targetElement.Data.TargetType,
                    EnumValue = targetElement.Data.EnumValue
                });
            }
        }
    }

    public void SetTargets(FixedList4096Bytes<TargetElement> targets){
        Targets.Clear();
        foreach (var targetElement in targets){
            if (targetElement.Data.TargetEntity != Entity.Null || !targetElement.Data.EnumValue.IsBlank()){
                Targets.Add(new TargetsGroupData(){
                    Entity = targetElement.Data.TargetEntity,
                    TargetType = targetElement.Data.TargetType,
                    EnumValue = targetElement.Data.EnumValue
                });
            }
        }
    }

    public void SetTargets(Entity self, ActionElement action, DynamicBuffer<TargetElement> targets){
        SetTargets(action, targets);
        AddTarget(TargetType.SELF, self);
    }

    public void SetTargets(Entity self, FixedList4096Bytes<ActiveActionTargetElement> targets){
        SetTargets(targets);
        AddTarget(TargetType.SELF, self);
    }

    public void SetTargets(Entity self, FixedList4096Bytes<TargetElement> targets){
        SetTargets(targets);
        AddTarget(TargetType.SELF, self);
    }

    public void SetSingleTarget(TargetType targetType, Entity targetEntity){
        Targets.Clear();
        Targets.Add(new TargetsGroupData(){
            TargetType = targetType,
            Entity = targetEntity
        });
    }

    public void OverwriteSingleTarget(TargetType targetType, Entity targetEntity){
        for (var i = 0; i < Targets.Length; i++){
            var targ = Targets[i];
            if (targ.TargetType == targetType){
                targ.Entity = targetEntity;
                Targets[i] = targ;
                return;
            }
        }

        AddTarget(targetType, targetEntity);
    }

    public void AddTarget(TargetType targetType, Entity targetEntity){
        Targets.Add(new TargetsGroupData(){
            TargetType = targetType,
            Entity = targetEntity
        });
    }

    public void AddTargets(FixedList4096Bytes<TargetElement> targets){
        foreach (var targetElement in targets){
            if (targetElement.Data.TargetEntity != Entity.Null || !targetElement.Data.EnumValue.IsBlank()){
                Targets.Add(new TargetsGroupData(){
                    Entity = targetElement.Data.TargetEntity,
                    TargetType = targetElement.Data.TargetType,
                    EnumValue = targetElement.Data.EnumValue
                });
            }
        }
    }

    public Entity GetPrimaryTargetEntity(ConditionData data){
        foreach (var targ in Targets){
            if (targ.TargetType == data.PrimaryTarget){
                return targ.Entity;
            }
        }

        return default;
    }

    public Entity GetSecondaryTargetEntity(ConditionData data){
        foreach (var targ in Targets){
            if (targ.TargetType == data.SecondaryTarget){
                return targ.Entity;
            }
        }

        return default;
    }

    public Entity GetTargetEntity(TargetType type){
        foreach (var targ in Targets){
            if (targ.TargetType == type){
                return targ.Entity;
            }
        }

        return default;
    }

    public FixedList4096Bytes<Entity> GetTargetEntities(TargetType type){
        var targets = new FixedList4096Bytes<Entity>();
        foreach (var targ in Targets){
            if (targ.TargetType == type){
                targets.Add(targ.Entity);
            }
        }

        return targets;
    }
    
    public DynamicGameEnum GetTargetEnum(TargetType type){
        foreach (var targ in Targets){
            if (targ.TargetType == type){
                return targ.EnumValue;
            }
        }

        return default;
    }
}

public struct TargetsGroupData {
    public TargetType TargetType;
    public Entity Entity;
    public DynamicGameEnum EnumValue;
    public FixedString32Bytes StringValue;
}