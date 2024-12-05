public struct GameTimeStamp {
    public int Day;
    public int Year;
    public TimeSeasonState SeasonState;
    public double TotalInGameSeconds;
    
    public bool MatchesSeasonAndYear(InGameTime inGameTime){
        return Year == inGameTime.Years && SeasonState == inGameTime.CurrentSeason;
    }
}