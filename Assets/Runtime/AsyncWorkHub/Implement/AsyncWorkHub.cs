/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AsyncWork.cs
 *  Description  :  Hub to manage works.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/20/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections.Generic;

namespace MGS.Work
{
    /// <summary>
    /// Hub to manage works.
    /// </summary>
    public class AsyncWorkHub : AsyncCruiser, IAsyncWorkHub
    {
        /// <summary>
        /// Max count of concurrency works.
        /// </summary>
        public int Concurrency { set; get; }

        /// <summary>
        /// Count of waitings works.
        /// </summary>
        public int Waitings { get { return waitingWorks.Count; } }

        /// <summary>
        /// Count of workings works.
        /// </summary>
        public int Workings { get { return workingWorks.Count; } }

        /// <summary>
        /// Resolver to check retrieable.
        /// </summary>
        public IRetryResolver Resolver { set; get; }

        /// <summary>
        /// Queue for waiting works.
        /// </summary>
        protected Queue<IAsyncWork> waitingWorks = new Queue<IAsyncWork>();

        /// <summary>
        /// List for working works.
        /// </summary>
        protected List<IAsyncWork> workingWorks = new List<IAsyncWork>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="interval">Interval of cruiser (ms).</param>
        /// <param name="concurrency">Max count of concurrency works.</param>
        /// <param name="resolver">Resolver to check retrieable.</param>
        public AsyncWorkHub(int interval = 250, int concurrency = 3, IRetryResolver resolver = null) :
            base(interval)
        {
            Concurrency = concurrency;
            Resolver = resolver;
        }

        /// <summary>
        /// Enqueue work to hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <returns></returns>
        public virtual IAsyncWork<T> Enqueue<T>(IAsyncWork<T> work)
        {
            waitingWorks.Enqueue(work);
            return work;
        }

        /// <summary>
        /// Clear cache resources.
        /// </summary>
        /// <param name="workings">Clear the working works?</param>
        /// <param name="waitings">Clear the waiting works?</param>
        public virtual void Clear(bool workings, bool waitings)
        {
            if (workings)
            {
                foreach (var work in workingWorks)
                {
                    work.AbortAsync();
                }
                workingWorks.Clear();
                Resolver?.Clear();
            }
            if (waitings)
            {
                waitingWorks.Clear();
            }
        }

        /// <summary>
        /// Cruiser tick every cycle.
        /// </summary>
        protected override void CruiserTick()
        {
            // Dequeue waitings to workings.
            while (waitingWorks.Count > 0 && workingWorks.Count < Concurrency)
            {
                var work = waitingWorks.Dequeue();
                if (work.IsDone)
                {
                    ClearRetryHistory(work);
                    OnWorkIsDone(work);
                    continue;
                }

                work.ExecuteAsync();
                workingWorks.Add(work);
            }

            // Check workings.
            for (int i = 0; i < workingWorks.Count; i++)
            {
                var work = workingWorks[i];
                if (work.IsDone)
                {
                    if (work.Error != null && CheckRetrieable(work))
                    {
                        work.ExecuteAsync();
                        continue;
                    }

                    workingWorks.RemoveAt(i);
                    ClearRetryHistory(work);
                    OnWorkIsDone(work);
                    i--;
                }
            }
        }

        /// <summary>
        /// On work is done.
        /// </summary>
        /// <param name="work"></param>
        protected virtual void OnWorkIsDone(IAsyncWork work)
        {
            ClearRetryHistory(work);
        }

        /// <summary>
        /// Check work is retrieable?
        /// </summary>
        /// <param name="work"></param>
        /// <returns></returns>
        protected bool CheckRetrieable(IAsyncWork work)
        {
            if (Resolver == null)
            {
                return false;
            }
            return Resolver.Retrieable(work);
        }

        /// <summary>
        /// Clear the retry history of work in resolver.
        /// </summary>
        /// <param name="work"></param>
        protected void ClearRetryHistory(IAsyncWork work)
        {
            Resolver?.Clear(work);
        }
    }
}