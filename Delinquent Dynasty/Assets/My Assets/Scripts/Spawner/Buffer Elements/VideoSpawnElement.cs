using System;
using Unity.Entities;

public struct VideoSpawnElement : IBufferElementData {
        public Guid GroupId;
        public Entity TargetEntity;
        public VideoFrameElement InitialFrame;
}