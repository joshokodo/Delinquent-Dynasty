using Unity.Entities;

public struct EventTimestamp {
    public Entity Source;
    public GameTimeStamp TimeStamp;

    public EventTimestamp(InGameTime inGameTime, Entity source){
        Source = source;
        TimeStamp.Day = inGameTime.Days;
        TimeStamp.Year = inGameTime.Years;
        TimeStamp.TotalInGameSeconds = inGameTime.TotalInGameSeconds;
        TimeStamp.SeasonState = inGameTime.CurrentSeason;
    }
}