using Unity.Entities;

public struct KnowledgeTimestamp {
    public Entity Source;
    public int Day;
    public int Year;
    public TimeSeasonState SeasonState;
    public double TotalInGameSeconds;

    public KnowledgeTimestamp(InGameTime inGameTime, Entity source){
        Source = source;
        Day = inGameTime.Days;
        Year = inGameTime.Years;
        TotalInGameSeconds = inGameTime.TotalInGameSeconds;
        SeasonState = inGameTime.CurrentSeason;
    }


    public bool MatchesSeasonAndYear(InGameTime inGameTime){
        return Year == inGameTime.Years && SeasonState == inGameTime.CurrentSeason;
    }
}