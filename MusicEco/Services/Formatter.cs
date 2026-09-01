using MusicEco.Core.Services;
using System.Globalization;

namespace MusicEco.Services;

public class TimeFormatter {
    private readonly AssemblyLocalization _l;
    public TimeFormatter(ILocalizationService localizationService) {
        _l = localizationService.Get(GetType());
    }
    public string Format(DateTime? dateTime) {
        return dateTime?.ToString("G", CultureInfo.CurrentUICulture) ?? string.Empty;
    }
    public string Format(TimeSpan time) {
        if (time.TotalHours > 1) {
            var format = _l["Format_Time_HourMinuteSecond"];
            return string.Format(format, (int)Math.Floor(time.TotalHours), time.Minutes, time.Seconds);
        }
        else if (time.TotalMinutes > 1) {
            var format = _l["Format_Time_MinuteSecond"];
            return string.Format(format, time.Minutes, time.Seconds);
        }
        else if (time.TotalSeconds > 0) {
            if (time.Seconds > 1) {
                var format = _l["Format_Time_Seconds"];
                return string.Format(format, time.Seconds);
            }
            else {
                var format = _l["Format_Time_Second"];
                return string.Format(format, time.Seconds);
            }
        }
        else {
            var format = _l["Format_Time_Second"];
            return string.Format(format, 0);
        }
    }
    public string Different(DateTime dateTime) {
        var diff = DateTime.UtcNow - dateTime;
        if (diff.TotalDays >= 365) {
            int year = (int)(diff.TotalDays / 365);
            if (year <= 1) {
                var format = _l["Format_Time_Year"];
                return string.Format(format, year);
            }
            else {
                var format = _l["Format_Time_Years"];
                return string.Format(format, year);
            }
        }
        if (diff.TotalDays >= 30) {
            int month = (int)(diff.TotalDays / 30);
            if (month <= 1) {
                var format = _l["Format_Time_Month"];
                return string.Format(format, month);
            }
            else {
                var format = _l["Format_Time_Months"];
                return string.Format(format, month);
            }
        }
        if (diff.TotalDays >= 1) {
            int day = (int)diff.TotalDays;
            if (day <= 1) {
                var format = _l["Format_Time_Day"];
                return string.Format(format, day);
            }
            else {
                var format = _l["Format_Time_Days"];
                return string.Format(format, day);
            }
        }
        if (diff.TotalHours >= 1) {
            int hour = (int)diff.TotalHours;
            if (hour <= 1) {
                var format = _l["Format_Time_Hour"];
                return string.Format(format, hour);
            }
            else {
                var format = _l["Format_Time_Hour"];
                return string.Format(format, hour);
            }
        }
        if (diff.TotalMinutes >= 1) {
            int minute = (int)diff.TotalMinutes;
            if (minute <= 1) {
                var format = _l["Format_Time_Minute"];
                return string.Format(format, minute);
            }
            else {
                var format = _l["Format_Time_Minutes"];
                return string.Format(format, minute);
            }
        }
        if (diff.TotalSeconds >= 1) {
            int second = (int)diff.TotalSeconds;
            if (second <= 1) {
                var format = _l["Format_Time_Second"];
                return string.Format(format, second);
            }
            else {
                var format = _l["Format_Time_Seconds"];
                return string.Format(format, second);
            }
        }
        return _l["Format_Time_Recent"];
    }
}

public class BasicFormatter {
    public string Format(string? text) {
        return text ?? string.Empty;
    }
    public string Format(IReadOnlyList<string> texts) {
        return string.Join(", ", texts);
    }
    public string Format(long? number) {
        return number?.ToString() ?? string.Empty;
    }
    public string Format(int? number) {
        return number?.ToString() ?? string.Empty;
    }
    public string Format(long? curr, long? total) {
        return $"{Format(curr)}/{Format(total)}";
    }
    public string Format(int? curr, int? total) {
        return $"{Format(curr)}/{Format(total)}";
    }
    public string Percent(float? number) {
        return ((number ?? 0f) * 100).ToString("F2");
    }
    public string Percent(double? number) {
        return ((number ?? 0f) * 100).ToString("F2");
    }

}