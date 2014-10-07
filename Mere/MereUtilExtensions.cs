using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mere
{
    public static class MereUtilExtensions
    {
        /// <summary>
        /// Extension to turn PropertyDescriptorCollection into a generic List of PropertyDescriptor
        /// </summary>
        /// <param name="propertyDescriptorCollection"></param>
        /// <returns></returns>
        public static List<PropertyDescriptor> PropertyDescriptorCollectionToList(this PropertyDescriptorCollection propertyDescriptorCollection)
        {
            return propertyDescriptorCollection.Cast<PropertyDescriptor>().ToList();
        }

        /// <summary>
        /// Converts linq expression to the string version of selected property's name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProp"></typeparam>
        /// <param name="src"></param>
        /// <returns>Returns property's name as string</returns>
        public static string GetPropName<T, TProp>(this Expression<Func<T, TProp>> src)
        {
            var pinfo = MereUtils.GetProperty(src);
            return pinfo.Name;
        }

        public static IEnumerable<TCopyTo> MereCopyTo<T, TCopyTo>(this IEnumerable<T> toCopy) where T : new() where TCopyTo : new()
        {
            var toCopyList = toCopy.ToList();

            var mereTableMinFrom = MereUtils.CacheCheck<T>();
            var mereTableMinTo = MereUtils.CacheCheck<TCopyTo>();

            
            foreach (var from in toCopyList)
            {
                var n = Activator.CreateInstance<TCopyTo>();
                foreach (var fromProp in mereTableMinFrom.SelectMereColumns)
                {
                    var mereColumn = mereTableMinTo.GetMereColumn(fromProp.ColumnName);

                    if (mereColumn == null)
                        continue;

                    mereColumn.SetBase(n, fromProp.GetBase(from));
                }
                yield return n;
            }

            //var toReturn = new List<TCopyTo>();
//            Parallel.For(0, toCopyList.Count,
//                i =>
//                {
//                    var n = Activator.CreateInstance<TCopyTo>();
//                    foreach (var fromProp in mereTableMinFrom.SelectMereColumns)
//                    {
//                        var mereColumn = mereTableMinTo.GetMereColumn(fromProp.ColumnName);
//
//                        if (mereColumn == null)
//                            continue;
//
//                        mereColumn.SetBase(n, fromProp.GetBase(toCopyList[i]));
//                    }
//                    toReturn.Add(n);
//                });
            //return toReturn;
        }

        #region int
        /// <summary>
        /// Checks if int is in range created using provided start and end values
        /// </summary>
        /// <param name="toCheck"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool InRange(this int toCheck, int start, int end)
        {
            return MereUtils.InRange(toCheck, start, end);
        }
        #endregion

        #region string
        /// <summary>
        /// Removes multiple spaces for strings
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static string RemoveMultipleSpaces(this string strIn)
        {
            return MereUtils.RemoveMultipleSpaces(strIn);
        }

        /// <summary>
        /// Encrypts string with salt
        /// </summary>
        /// <param name="plain"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Encrypt(this string plain, string salt = "KX1c#Vb5Gt6l@")
        {
            return MereUtils.Encrypt(plain, salt);
        }

        /// <summary>
        /// Splits string on spaces
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="removeEmpties"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitAndStripOnSpaces(this string strIn, bool removeEmpties = false)
        {
            return MereUtils.SplitAndStripOnSpaces(strIn, removeEmpties);
        }

        /// <summary>
        /// Splits string on spaces
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="removeEmpties"></param>
        /// <returns>IEnumerable after spliting and making lowercase</returns>
        public static IEnumerable<string> SplitAndStripOnSpacesToLower(this string strIn, bool removeEmpties = false)
        {
            return MereUtils.SplitAndStripOnSpacesToLower(strIn, removeEmpties);
        }

        public static IEnumerable<string> SplitOnNewLine(this string strIn, bool removeEmpties = false)
        {
            return MereUtils.SplitOnNewLine(strIn, removeEmpties);
        }
        #endregion
        
        #region DateTime
        public static double ToJulian(this DateTime now)
        {
            return MereUtils.JulianDate(now);
        }

        public static int GetQuarter(this DateTime now)
        {
            return MereUtils.GetQuarter(now);
        }

        public static int GetLastQuarter(this DateTime now)
        {
            return MereUtils.GetLastQuarter(now);
        }

        public static int GetNextQuarter(this DateTime now)
        {
            return MereUtils.GetNextQuarter(now);
        }

        public static bool IsQuarter(this DateTime now, int quarter)
        {
            return MereUtils.IsQuarter(now, quarter);
        }

        public static bool IsFirstQuarter(this DateTime now)
        {
            return now.IsQuarter(1);
        }

        public static bool IsSecondQuarter(this DateTime now)
        {
            return now.IsQuarter(2);
        }

        public static bool IsThirdQuarter(this DateTime now)
        {
            return now.IsQuarter(3);
        }

        public static bool IsFourthQuarter(this DateTime now)
        {
            return now.IsQuarter(4);
        }

        public static DateTime GetFirstDayLastFirstQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayLastFirstQuarter(now);
        }

        public static DateTime GetFirstDayLastSecondQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayLastSecondQuarter(now);
        }

        public static DateTime GetFirstDayLastThirdQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayLastThirdQuarter(now);
        }

        public static DateTime GetFirstDayLastFourthQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayLastFourthQuarter(now);
        }

        public static DateTime GetFirstDayNextFirstQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayNextFirstQuarter(now);
        }

        public static DateTime GetFirstDayNextSecondQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayNextSecondQuarter(now);
        }

        public static DateTime GetFirstDayNextThirdQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayNextThirdQuarter(now);
        }

        public static DateTime GetFirstDayNextFourthQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayNextFourthQuarter(now);
        }

        public static DateTime GetLastDayLastFirstQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayLastFirstQuarter(now);
        }

        public static DateTime GetLastDayLastSecondQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayLastSecondQuarter(now);
        }

        public static DateTime GetLastDayLastThirdQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayLastThirdQuarter(now);
        }

        public static DateTime GetLastDayLastFourthQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayLastFourthQuarter(now);
        }

        public static DateTime GetLastDayNextFirstQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayNextFirstQuarter(now);
        }

        public static DateTime GetLastDayNextSecondQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayNextSecondQuarter(now);
        }

        public static DateTime GetLastDayNextThirdQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayNextThirdQuarter(now);
        }

        public static DateTime GetLastDayNextFourthQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayNextFourthQuarter(now);
        }

        //
        public static DateTime GetDayByOccurrenceInMonth(this DateTime startDate, DayOfWeek day, int occurrence)
        {
            return MereUtils.GetDayByOccurrenceInMonth(startDate, day, occurrence);
        }

//        public static DateTime GetDayByOccurrenceInMonthFromFirst(this DateTime now, DayOfWeek day, int occurrence)
//        {
//            return MereUtils.GetDayByOccurrenceInMonthFromFirst(now, day, occurrence);
//        }

        public static DateTime GetFirstDayOfLastMonth(this DateTime now)
        {
            return MereUtils.GetFirstDayOfLastMonth(now);
        }

        public static DateTime GetFirstDayOfMonth(this DateTime now)
        {
            return MereUtils.GetFirstDayOfMonth(now);
        }

        public static DateTime GetLastDayOfLastMonth(this DateTime now)
        {
            return MereUtils.GetLastDayOfLastMonth(now);
        }

        public static DateTime GetLastDayOfMonth(this DateTime now)
        {
            return MereUtils.GetLastDayOfMonth(now);
        }

        public static DateTime GoToPrevMonth(this DateTime now, int month)
        {
            return MereUtils.GoToPrevMonth(now, month);
        }

        public static DateTime GoToNextMonth(this DateTime now, int month)
        {
            return MereUtils.GoToNextMonth(now, month);
        }

        //
        public static DateTime GetFirstDayOfLastYear(this DateTime now)
        {
            return MereUtils.GetFirstDayOfLastYear(now);
        }

        public static DateTime GetFirstDayOfYear(this DateTime now)
        {
            return MereUtils.GetFirstDayOfYear(now);
        }

        public static DateTime GetLastDayOfLastYear(this DateTime now)
        {
            return MereUtils.GetLastDayOfLastYear(now);
        }

        public static DateTime GetLastDayOfYear(this DateTime now)
        {
            return MereUtils.GetLastDayOfYear(now);
        }
        //

        public static DateTime GetFirstDayLastQuarter(this DateTime now)
        {
            return MereUtils.GetFirstDayLastQuarter(now);
        }

        public static DateTime GetLastDayLastQuarter(this DateTime now)
        {
            return MereUtils.GetLastDayLastQuarter(now);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime MereGetPrevWeeksDayOfWeekDate(this DateTime startDate, DayOfWeek moveTo, int weeksAway)
        {
            return MereUtils.GetPrevWeeksDayOfWeekDate(startDate, moveTo, weeksAway);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime MereGetPrevWeeksDayOfWeekDate(this DateTime startDate, DayOfWeek moveTo)
        {
            return MereUtils.GetPrevWeeksDayOfWeekDate(startDate, moveTo, 1);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime MereGetPrevWeeksDayOfWeekDate(this DateTime startDate, string moveTo, int weeksAway)
        {
            return MereUtils.GetPrevWeeksDayOfWeekDate(startDate, moveTo, weeksAway);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime MereGetNextWeeksDayOfWeekDate(this DateTime startDate, DayOfWeek moveTo, int weeksAway)
        {
            return MereUtils.GetNextWeeksDayOfWeek(startDate, moveTo, weeksAway);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime MereGetNextWeeksDayOfWeekDate(this DateTime startDate, DayOfWeek moveTo)
        {
            return MereUtils.GetNextWeeksDayOfWeek(startDate, moveTo, 1);
        }

        /// <summary>
        /// Gets date for instance of day next week.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <param name="weeksAway">How many weeks away to find date 0 = next instance</param>
        /// <returns></returns>
        public static DateTime MereGetNextWeeksDayOfWeekDate(this DateTime startDate, string moveTo, int weeksAway)
        {
            return MereUtils.GetNextWeeksDayOfWeek(startDate, moveTo, weeksAway);
        }

        /// <summary>
        /// Gets date for previous instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime MereGetPrevDayOfWeekDate(this DateTime startDate, DayOfWeek moveTo)
        {
            return MereUtils.GetPrevDayOfWeek(startDate, moveTo);
        }

        /// <summary>
        /// Gets date for previous instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">Day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime MereGetPrevDayOfWeekDate(this DateTime startDate, string moveTo)
        {
            return MereUtils.GetPrevDayOfWeek(startDate, moveTo);
        }

        /// <summary>
        /// Gets date for next instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">String version of day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime MereGetNextDayOfWeekDate(this DateTime startDate, string moveTo)
        {
            return MereUtils.GetNextDayOfWeek(startDate, moveTo);
        }

        /// <summary>
        /// Gets date for next instance of day.
        /// </summary>
        /// <param name="startDate">Date to start at.</param>
        /// <param name="moveTo">Day of the week to move to.</param>
        /// <returns></returns>
        public static DateTime MereGetNextDayOfWeekDate(this DateTime startDate, DayOfWeek moveTo)
        {
            return MereUtils.GetNextDayOfWeek(startDate, moveTo);
        }
        #endregion
    }
}
