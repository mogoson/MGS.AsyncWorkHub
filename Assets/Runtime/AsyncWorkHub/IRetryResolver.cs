/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  IRetryResolver.cs
 *  Description  :  Interface of resolver to check work retrieable.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/20/2022
 *  Description  :  Initial development version.
 *************************************************************************/

namespace MGS.Work
{
    /// <summary>
    /// Interface of resolver to check work retrieable.
    /// </summary>
    public interface IRetryResolver
    {
        /// <summary>
        /// Check check is retrieable?
        /// </summary>
        /// <param name="work"></param>
        /// <returns></returns>
        bool Retrieable(IAsyncWork work);

        /// <summary>
        /// Clear the history of work.
        /// </summary>
        /// <param name="work"></param>
        void Clear(IAsyncWork work);

        /// <summary>
        /// Clear the history of all works.
        /// </summary>
        void Clear();
    }
}