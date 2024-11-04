using Unity.Entities;

public struct DailyEventsKnowledgeComponent : IComponentData {
    public int Day;
    public TimeSeasonState SeasonState;
    public int Year;

    public bool MatchesTime(InGameTime inGameTime){
        return Day == inGameTime.Days && SeasonState == inGameTime.CurrentSeason && Year == inGameTime.Years;
    }
}