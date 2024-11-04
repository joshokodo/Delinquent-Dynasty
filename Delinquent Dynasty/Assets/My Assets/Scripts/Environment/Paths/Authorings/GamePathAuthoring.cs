using Unity.Entities;
using UnityEngine;

public class GamePathAuthoring : MonoBehaviour { }

public class GamePathBaker : Baker<GamePathAuthoring> {
    public override void Bake(GamePathAuthoring authoring){
        AddBuffer<PathPointElement>();
    }
}