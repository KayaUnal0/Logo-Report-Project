using Common.Shared.Enums;
using System;

public static class CronUtils
{
    public static string DailyCron(TimeSpan time)
    {
        return $"{time.Minutes} {time.Hours} * * *";
    }
    public static string WeeklyCron(this WeekDay day, TimeSpan time)
    {
        var cronDay = (int)day;

        return $"{time.Minutes} {time.Hours} * * {cronDay}";
    }

    public static string MonthlyCron(TimeSpan time, int dayOfMonth)
    {
        return $"{time.Minutes} {time.Hours} {dayOfMonth} * *";
    }
}
