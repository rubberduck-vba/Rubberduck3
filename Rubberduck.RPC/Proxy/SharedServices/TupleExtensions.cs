namespace Rubberduck.RPC.Proxy.SharedServices
{
    public static class TupleExtensions
    {
        /// <summary>
        /// Provides support for try-pattern over <c>Task&lt;(bool,T)&gt;</c> for async methods that return such a tuple..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="outcome"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryOut<T>(this (bool success, T result) outcome, out T result)
        {
            result = outcome.result;
            return outcome.success;
        }
    }
}