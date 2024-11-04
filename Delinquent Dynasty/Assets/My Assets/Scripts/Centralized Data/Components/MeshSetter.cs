using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshSetter : MonoBehaviour {
    public static MeshSetter Instance;

    [FormerlySerializedAs("MaleStudentHeads")]
    public List<CharacterBodyPartMeshData> MaleHeads = new();

    public List<CharacterBodyPartMeshData> MaleTorso = new();
    public List<CharacterBodyPartMeshData> MaleLegs = new();
    public List<CharacterBodyPartMeshData> MaleShoes = new();

    [FormerlySerializedAs("FemaleStudentHeads")]
    public List<CharacterBodyPartMeshData> FemaleHeads = new();

    public List<CharacterBodyPartMeshData> FemaleTorso = new();
    public List<CharacterBodyPartMeshData> FemaleLegs = new();
    public List<CharacterBodyPartMeshData> FemaleShoes = new();

    public List<Material> SkinMaterials = new();
    public List<Material> HairMaterials = new();
    public List<Material> ClothesMaterials = new();

    public void Awake(){
        Instance = this;
    }
}

[Serializable]
public class CharacterBodyPartMeshData {
    public Mesh Mesh;
    public List<CharacterMaterialType> MaterialOrder;


    public void SetSkinnedRenderer(SkinnedMeshRenderer renderer, Material skin, Material hair, Material primary,
        Material secondary, Material tertiary){
        renderer.sharedMesh = Mesh;
        var materials = new List<Material>();
        MaterialOrder.ForEach(m => {
            switch (m){
                case CharacterMaterialType.HAIR:
                    if (hair != null){
                        materials.Add(hair);
                    }

                    break;
                case CharacterMaterialType.SKIN:
                    if (skin != null){
                        materials.Add(skin);
                    }

                    break;
                case CharacterMaterialType.PRIMARY:
                    if (primary != null){
                        materials.Add(primary);
                    }

                    break;
                case CharacterMaterialType.SECONDARY:
                    if (secondary != null){
                        materials.Add(secondary);
                    }

                    break;
                case CharacterMaterialType.TERTIARY:
                    if (tertiary != null){
                        materials.Add(tertiary);
                    }

                    break;
            }
        });
        renderer.SetSharedMaterials(materials);
    }
}

public enum CharacterMaterialType {
    NONE,
    SKIN,
    HAIR,
    PRIMARY,
    SECONDARY,
    TERTIARY,
}