﻿/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  IAsyncWorkHub.cs
 *  Description  :  Interface of hub to manage works.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/20/2022
 *  Description  :  Initial development version.
 *************************************************************************/

namespace MGS.Work
{
    /// <summary>
    /// Interface of hub to manage works.
    /// </summary>
    public interface IAsyncWorkHub
    {
        /// <summary>
        /// Max count of concurrency works.
        /// </summary>
        int Concurrency { set; get; }

        /// <summary>
        /// Count of waitings works.
        /// </summary>
        int Waitings { get; }

        /// <summary>
        /// Count of workings works.
        /// </summary>
        int Workings { get; }

        /// <summary>
        /// Resolver to check work retrieable.
        /// </summary>
        IWorkResolver Resolver { set; get; }

        /// <summary>
        /// Enqueue work to hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <returns></returns>
        IAsyncWork<T> EnqueueWork<T>(IAsyncWork<T> work);

        /// <summary>
        /// Clear cache resources.
        /// </summary>
        /// <param name="workings">Clear the working works?</param>
        /// <param name="waitings">Clear the waiting works?</param>
        void Clear(bool workings, bool waitings);

        /// <summary>
        /// Abort async operations.
        /// </summary>
        void Abort();
    }
}