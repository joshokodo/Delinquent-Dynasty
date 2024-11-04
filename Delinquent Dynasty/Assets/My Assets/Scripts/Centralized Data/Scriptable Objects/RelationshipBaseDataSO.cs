using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RELATIONSHIP_EFFECT_", menuName = "Scriptables/Relationships/Relationship Effect")]
public class RelationshipBaseDataSO : ScriptableObject {
    public string relationshipDescription;

    [FormerlySerializedAs("relationshipTitleType")]
    public RelationshipMainTitleType relationshipMainTitleType;

    public List<PassiveEffectsAndConditionsDTO> passiveEffects;
}