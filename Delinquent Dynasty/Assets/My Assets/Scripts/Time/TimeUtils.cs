using System;
using Unity.Collections;

public struct TimeUtils {
    private static FixedString64Bytes dateFormat = new("Day {0} of {1}, Year {2}");

    public static FixedString128Bytes GetGameDateString(KnowledgeTimestamp timestamp){
        return GetGameDateString(timestamp.Day, timestamp.SeasonState, timestamp.Year);
    }
    
    public static FixedString128Bytes GetGameDateString(int days, TimeSeasonState seasonState, int years){
        return new FixedString128Bytes(string.Format(dateFormat.Value, (days + 1).ToString(), seasonState.ToString(),
            years));
    }

    public static FixedString128Bytes GetGameTimeString(double secs){
        return new FixedString128Bytes(new DateTime().AddSeconds(secs).ToString("hh:mm tt"));
    }

    public static FixedString32Bytes GetGameTimeSpanString(double secs){
        return new FixedString32Bytes(new TimeSpan(0, 0, 0, (int) secs).ToString("c"));
    }
}