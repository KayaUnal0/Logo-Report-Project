using Common.Shared.Enums;
using System;

public static class CronUtils
{
    public static string DailyCron(TimeSpan time)
    {
        return $"{time.Minutes} {time.Hours} * * *";
    }
    public static string WeeklyCron(List<WeekDay> weekDays, TimeSpan time)
    {
        var selectedDays = weekDays.Select(day => ((int)day).ToString());

        var cronDays = string.Join(",", selectedDays);

        return $"{time.Minutes} {time.Hours} * * {cronDays}";
    }

    public static string MonthlyCron(TimeSpan time, int dayOfMonth)
    {
        return $"{time.Minutes} {time.Hours} {dayOfMonth} * *";
    }
}
