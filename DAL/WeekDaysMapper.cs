using System.Collections.Generic;

namespace Flights.DAL
{
    /// <summary>
    /// Defines days of the week in the following format:
    /// Monday=1, Tuesday=2, Wednesday=4, Thursday=8, Friday=16, Saturday=32, Sunday=64. 
    /// Specifically, this class is needed for remapping week days from DateTimeOffset.DayOfWeek to required format.
    /// </summary>
    internal static class WeekDaysMapper
    {
        /// <summary/>
        public static readonly Dictionary<int, int> weekDays = new Dictionary<int, int>(){
            [1]=1,
            [2]=2,
            [3]=4,
            [4]=8,
            [5]=16,
            [6]=32,
            [0]=64
        };
    }
}
