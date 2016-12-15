namespace HIT.Web.Infrastructure.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// If a number has single digit (7 for example) - returns a string in the format "07"
        /// If a number has multiple digits (10 for example) - returns a string in the format "10"
        /// </summary>
        /// <param name="number">The number to convert</param>
        /// <returns>The number as time string</returns>        
        public static string ToTimeString(this int number)
        {
            var isSingleDigit = number < 10;
            var result = isSingleDigit ? "0" + number.ToString() : number.ToString();

            return result;
        }
    }
}
