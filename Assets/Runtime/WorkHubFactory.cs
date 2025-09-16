/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  WorkHubFactory.cs
 *  Description  :  Factory for create async work hub.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/22/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using MGS.Cachers;

namespace MGS.Work
{
    /// <summary>
    /// Factory for create async work hub.
    /// </summary>
    public static class WorkHubFactory
    {
        /// <summary>
        /// Create hub with concurrency and retry ability.
        /// </summary>
        /// <param name="interval">Interval of cruiser (ms).</param>
        /// <param name="concurrency">Max count of concurrency works.</param>
        /// <param name="retryTimes">Retry times. do not active retry ability if let retryTimes=0.</param>
        /// <param name="tolerables">Tolerable exception types can be retry. default is [WebException,TimeoutException] if let it null.</param>
        /// <returns></returns>
        public static IAsyncWorkHub CreateHub(int interval = 250, int concurrency = 10,
            int retryTimes = 3, ICollection<Type> tolerables = null)
        {
            var resolver = CreateResolver(retryTimes, tolerables);
            return new AsyncWorkHub(interval, concurrency, resolver);
        }

        /// <summary>
        /// Create cache hub with concurrency and cache ability.
        /// </summary>
        /// <param name="interval">Interval of cruiser (ms).</param>
        /// <param name="concurrency">Max count of concurrency works.</param>
        /// <param name="retryTimes">Retry times. do not active retry ability if let retryTimes=0.</param>
        /// <param name="tolerables">Tolerable exception types can be retry. default is [WebException,TimeoutException] if let it null.</param>
        /// <param name="maxCacheCount">Max count of caches.</param>
        /// <param name="cacheTimeout">Timeout(ms)</param>
        /// <returns></returns>
        public static IAsyncWorkCacheHub CreateCacheHub(int interval = 250, int concurrency = 10,
            int retryTimes = 3, ICollection<Type> tolerables = null,
            int maxCacheCount = 100, int cacheTimeout = 5000)
        {
            var resultCacher = CreateCacher<object>(maxCacheCount, cacheTimeout);
            var workCacher = CreateCacher<IAsyncWork>(maxCacheCount);
            var resolver = CreateResolver(retryTimes, tolerables);
            return new AsyncWorkCacheHub(resultCacher, workCacher, interval, concurrency, resolver);
        }

        /// <summary>
        /// Create status hub with concurrency and cache ability.
        /// </summary>
        /// <param name="interval">Interval of cruiser (ms).</param>
        /// <param name="concurrency">Max count of concurrency works.</param>
        /// <param name="retryTimes">Retry times. do not active retry ability if let retryTimes=0.</param>
        /// <param name="tolerables">Tolerable exception types can be retry. default is [WebException,TimeoutException] if let it null.</param>
        /// <param name="maxCacheCount">Max count of caches.</param>
        /// <param name="cacheTimeout">Timeout(ms)</param>
        /// <returns></returns>
        public static IAsyncWorkStatusHub CreateStatusHub(int interval = 250, int concurrency = 10,
            int retryTimes = 3, ICollection<Type> tolerables = null,
            int maxCacheCount = 100, int cacheTimeout = 5000)
        {
            var resultCacher = CreateCacher<object>(maxCacheCount, cacheTimeout);
            var workCacher = CreateCacher<IAsyncWork>(maxCacheCount);
            var resolver = CreateResolver(retryTimes, tolerables);

            //Thread async work, invoke the NotifyStatus method to notify status in your main thread.
            return new AsyncWorkStatusHub(resultCacher, workCacher, interval, concurrency, resolver);
        }

        /// <summary>
        /// Create status hub with concurrency and cache ability.
        /// </summary>
        /// <param name="interval">Interval of cruiser (ms).</param>
        /// <param name="concurrency">Max count of concurrency works.</param>
        /// <param name="retryTimes">Retry times. do not active retry ability if let retryTimes=0.</param>
        /// <param name="tolerables">Tolerable exception types can be retry. default is [WebException,TimeoutException] if let it null.</param>
        /// <param name="maxCacheCount">Max count of caches.</param>
        /// <param name="cacheTimeout">Timeout(ms)</param>
        /// <returns></returns>
        public static IAsyncWorkMonoHub CreateMonoHub(int interval = 250, int concurrency = 10,
            int retryTimes = 3, ICollection<Type> tolerables = null,
            int maxCacheCount = 100, int cacheTimeout = 5000)
        {
            var resultCacher = CreateCacher<object>(maxCacheCount, cacheTimeout);
            var workCacher = CreateCacher<IAsyncWork>(maxCacheCount);
            var resolver = CreateResolver(retryTimes, tolerables);

            //Thread async work, notify status in unity main thread.
            return new AsyncWorkMonoHub(resultCacher, workCacher, interval, concurrency, resolver);
        }

        /// <summary>
        /// Create resolver for check work retrieable.
        /// </summary>
        /// <param name="retryTimes">Retry times. do not active retry ability if let retryTimes=0.</param>
        /// <param name="tolerables">Tolerable exception types can be retry. default is [WebException,TimeoutException] if let it null.</param>
        /// <returns></returns>
        public static IRetryResolver CreateResolver(int retryTimes, ICollection<Type> tolerables)
        {
            if (retryTimes <= 0)
            {
                return null;
            }

            //Set tolerable exceptions to WorkResolver to check retrieable.
            if (tolerables == null)
            {
                tolerables = new List<Type> { typeof(WebException), typeof(TimeoutException) };
            }
            return new RetryResolver(retryTimes, tolerables);
        }

        /// <summary>
        /// Create cacher for work.
        /// </summary>
        /// <param name="maxCacheCount">Max count of caches.</param>
        /// <returns></returns>
        public static ICacher<T> CreateCacher<T>(int maxCacheCount)
        {
            if (maxCacheCount <= 0)
            {
                return null;
            }

            //A cacher to cache the waiting and working work to reuse for the same url.
            return new Cacher<T>(maxCacheCount);
        }

        /// <summary>
        /// Create cacher for work.
        /// </summary>
        /// <param name="maxCacheCount">Max count of caches.</param>
        /// <param name="cacheTimeout">Timeout(ms)</param>
        /// <returns></returns>
        public static ICacher<T> CreateCacher<T>(int maxCacheCount, int cacheTimeout)
        {
            if (maxCacheCount <= 0)
            {
                return null;
            }

            //A cacher with timeout to cache the result from work.
            return new TimeoutCacher<T>(maxCacheCount, cacheTimeout);
        }
    }
}