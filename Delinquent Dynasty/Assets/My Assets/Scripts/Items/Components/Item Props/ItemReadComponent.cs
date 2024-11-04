using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

//TODO: this and oothers like consume dont really need components. only thing nice is you can check if an entity
// has item prop without item store but is that important? consider removing and only having components for stuff that will change like decay
public struct ItemReadComponent : IComponentData { }