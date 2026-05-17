// --
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouJu.Infrastructure
{
    /// <summary>
    /// 时间扩展操作类
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取指定日期的年月日
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string YearMonthDayFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 获取指定日期的小时和分钟
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string HourMinuteFormat(this DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }
        /// <summary>
        /// 获取指定日期的年月
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string YearMonthFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM");
        }
        /// <summary>
        /// 得到完整日期
        /// </summary>
        /// <param name="dateTine"></param>
        /// <returns></returns>
        public static string GetFullDateFormat(this DateTime dateTine)
        {
          return dateTine.ToString("yyyy-MM-dd HH:mm:ss");
        }
     



        /// <summary>
        /// 当前时间是否周末
        /// </summary>
        /// <param name="dateTime">时间点</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weeks.Contains(dateTime.DayOfWeek);
        }

        /// <summary>
        /// 当前时间是否工作日
        /// </summary>
        /// <param name="dateTime">时间点</param>
        /// <returns></returns>
        public static bool IsWeekday(this DateTime dateTime)
        {
            DayOfWeek[] weeks = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weeks.Contains(dateTime.DayOfWeek);
        }


        /// <summary>
        /// 获取指定时间是星期几 中文
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string GetDayOfWeek(this DateTime dateTime)
        {
            return DateTime.Now.ToString("dddd");
        }

        /// <summary>
        /// 获取时间的0点到23点59分59秒 返回一个二位数组
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime[] GetDateTimeRange(this DateTime dateTime)
        { 
           return new DateTime[]{ DateTime.Parse(dateTime.YearMonthDayFormat()+" 00:00:00"),DateTime.Parse(dateTime.YearMonthDayFormat() + " 23:59:59")};          
        }

        /// <summary>
        /// 获取今天的范围
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetTodayRange()
        {
            var nowDate = DateTime.Now;
            return new DateTime[] { DateTime.Parse(nowDate.YearMonthDayFormat() + " 00:00:00"), DateTime.Parse(nowDate.YearMonthDayFormat() + " 23:59:59") };
        }
        /// <summary>
        /// 获取昨天的时间范围
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetTomorrowRange()
        {
            var tomorrowDate = DateTime.Now.AddDays(1);
            
            return new DateTime[] { DateTime.Parse(tomorrowDate.YearMonthDayFormat() + " 00:00:00"), DateTime.Parse(tomorrowDate.YearMonthDayFormat() + " 23:59:59") };
        }
        /// <summary>
        /// 获取昨天的时间范围
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetYesterdayRange()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            return new DateTime[] { DateTime.Parse(yesterday.YearMonthDayFormat() + "00:00:00"), DateTime.Parse(yesterday.YearMonthDayFormat() + "23:59:59") };
        }

        /// <summary>
        /// 获取最近n天时间范围
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetYesterdayRange(int n)
        {
            var start = DateTime.Now.AddDays(-n);
            var end = DateTime.Now.AddDays(n);
            return new DateTime[] { DateTime.Parse(start.YearMonthDayFormat() + "00:00:00"), DateTime.Parse(end.YearMonthDayFormat() + "23:59:59") };
        }

        /// <summary>
        /// 获取某个月的月初和月底时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime[] GetMonthRange(this DateTime dateTime)
        {
            var start = DateTime.Parse(dateTime.YearMonthFormat());
            var end = DateTime.Parse(dateTime.AddMonths(1).YearMonthFormat()).AddMilliseconds(-1);
            return new DateTime[] { start, end };
        }
        /// <summary>
        /// 获取当月的月初和月底时间
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetToMonthRange()
        {
            return GetMonthRange(DateTime.Now);
        }

        /// <summary>
        /// 获取本年度
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetToYearQuarterRange()
        {
          var start= DateTime.Parse(DateTime.Now.ToString("yyyy-01-01"));
          var end= DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddYears(1).AddDays(-1);
            return new DateTime[]{start,end};
        }
        /// <summary>
        /// 获取上年度
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetToYesteryearQuarterRange()
        {
            var start = DateTime.Parse(DateTime.Now.AddYears(-1).ToString("yyyy-01-01"));
            var end = DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddDays(-1);
            return new DateTime[] { start, end };
        }



        /// <summary>
        /// 根据时间返回几个月前,几天前,几小时前,几分钟前,几秒前
        /// </summary>
        /// <returns></returns>
        public static string DateBeforeNow(this DateTime date)
        {
            TimeSpan span = DateTime.Now - date;
            if (span.TotalDays > 60)
            {
                return date.ToShortDateString();
            }
            else if (span.TotalDays > 30)
            {
                return "1个月前";
            }
            else if (span.TotalDays > 14)
            {
                return "2周前";
            }
            else if (span.TotalDays > 7)
            {
                return "1周前";
            }
            else if (span.TotalDays > 1)
            {
                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
            }
            else if (span.TotalHours > 1)
            {
                return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                return "1秒前";
            }
        }


        /// <summary>
        /// 根据出生年月进行生日提醒
        /// </summary>
        /// <returns></returns>
        public static string BirthdayMessage(DateTime date)
        {
            DateTime now = DateTime.Now;
            //TimeSpan span = DateTime.Now - birthday;
            int nowMonth = now.Month;
            int birtMonth = date.Month;
            if (nowMonth == 12 && birtMonth == 1)
                return string.Format("下月{0}号", date.Day);
            if (nowMonth == 1 && birtMonth == 12)
                return string.Format("上月{0}号", date.Day);
            int months = now.Month - date.Month;
            //int days = now.Day - birthday.Day;
            if (months == 1)
                return string.Format("上月{0}号", date.Day);
            else if (months == -1)
                return string.Format("下月{0}号", date.Day);
            else if (months == 0)
            {
                if (now.Day == date.Day)
                    return "今天";
                return string.Format("本月{0}号", date.Day);
            }
            else if (months > 1)
                return string.Format("已过{0}月", months);
            else
                return string.Format("{0}月{1}日", date.Month, date.Day);
        }


        public static string GetFullDateFormat(this DateTime? date)
        {
            if (date.HasValue) return GetFullDateFormat(date.Value);
            return "";
        }
    }
}
