using Unity.Entities;

public struct AssignmentDocumentComponent : IComponentData {
    public Entity Author;
    public Entity Assignment;
    public int CorrectAnswers;
    public int WrongAnswers;
    public int NumberOfProblems;
}