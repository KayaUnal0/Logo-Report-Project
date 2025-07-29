using Common.Shared.Enums;

public static class CronUtils
{
    public static string ToCron(this WeekDay day)
    {
        return day switch
        {
            WeekDay.Pazartesi => "0 9 * * 1",
            WeekDay.Salı => "0 9 * * 2",
            WeekDay.Çarşamba => "0 9 * * 3",
            WeekDay.Perşembe => "0 9 * * 4",
            WeekDay.Cuma => "0 9 * * 5",
            WeekDay.Cumartesi => "0 9 * * 6",
            WeekDay.Pazar => "0 9 * * 0",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
