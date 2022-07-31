namespace North.Common
{
    public class TimeHelper
    {
        /// <summary>
        /// Unix 时间戳
        /// </summary>
        public static long TimeStamp
        {
            get
            {
                return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }
    }
}
