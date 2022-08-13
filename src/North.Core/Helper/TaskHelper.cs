namespace North.Core.Helper
{
    /// <summary>
    /// 异步操作辅助类
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// 带有超时处理的异步任务等待
        /// </summary>
        /// <typeparam name="TResult">异步任务返回值</typeparam>
        /// <param name="task">待执行的异步任务</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        /// <exception cref="TimeoutException"></exception>
        public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
            if (await Task.WhenAny(task, delayTask) == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task;
            }
            throw new TimeoutException("Operation time out");
        }
    }
}
