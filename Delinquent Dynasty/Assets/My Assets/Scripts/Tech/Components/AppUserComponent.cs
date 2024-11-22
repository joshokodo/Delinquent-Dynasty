using Unity.Collections;
using Unity.Entities;
using UnityEditor.ProjectWindowCallback;

public struct AppUserComponent : IComponentData {
    public Entity User;
    public bool FriendsOnlyView;
    public FixedList4096Bytes<Entity> Followers;
    public FixedList4096Bytes<Entity> Following;
}