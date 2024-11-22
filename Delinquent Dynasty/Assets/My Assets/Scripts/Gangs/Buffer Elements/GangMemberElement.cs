using Unity.Entities;
using UnityEditor.ProjectWindowCallback;

public struct GangMemberElement : IBufferElementData {
    public Entity Character;
    public int Rank;
}