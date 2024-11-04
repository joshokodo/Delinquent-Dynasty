using Unity.Entities;
using UnityEngine;

// todo probably delete
public readonly partial struct StudentAspect : IAspect {
    public readonly RefRW<StudentBio> StudentBio;
    public readonly RefRW<CharacterBio> CharacterBio;

    public bool IsGoodFriendMatch(StudentAspect studentAspect){
        return true;
    }
}