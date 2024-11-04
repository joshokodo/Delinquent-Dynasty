using System;

[Serializable]
public class InterestSpawnDTO {
    public int InitialValue;
    public InterestSubjectType SubjectType;
    public DynamicGameEnumDTO EnumValue;

    public InterestSpawnData ToData(){
        return new InterestSpawnData(){
            InitialValue = InitialValue,
            SubjectType = SubjectType,
            EnumValue = EnumValue.ToData()
        };
    }
}