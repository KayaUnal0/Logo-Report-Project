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
        var cronDay = day switch
        {
            WeekDay.Pazar => 0,
            WeekDay.Pazartesi => 1,
            WeekDay.Salı => 2,
            WeekDay.Çarşamba => 3,
            WeekDay.Perşembe => 4,
            WeekDay.Cuma => 5,
            WeekDay.Cumartesi => 6,
            _ => throw new ArgumentOutOfRangeException()
        };

        return $"{time.Minutes} {time.Hours} * * {cronDay}";
    }

    public static string MonthlyCron(TimeSpan time, int dayOfMonth)
    {
        return $"{time.Minutes} {time.Hours} {dayOfMonth} * *";
    }
}
