/*************************************************************************
 *  Copyright © 2023 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  AsyncWorkMonoHub.cs
 *  Description  :  Hub to manage work and cache data,
 *                  and unity main thread notify status.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  03/10/2023
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Collections;
using MGS.Cachers;
using UnityEngine;

namespace MGS.Work
{
    /// <summary>
    /// Hub to manage work and cache data,
    /// and main thread notify status.
    /// </summary>
    public class AsyncWorkMonoHub : AsyncWorkStatusHub, IAsyncWorkMonoHub
    {
        /// <summary>
        /// WorkHubBehaviour to handle MonoBehaviour.
        /// </summary>
        protected class WorkHubBehaviour : MonoBehaviour { }

        /// <summary>
        /// MonoBehaviour for hub to StartCoroutine.
        /// </summary>
        protected WorkHubBehaviour notifier;

        /// <summary>
        /// Yield Instruction for notifier tick.
        /// </summary>
        public YieldInstruction Instruction { set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resultCacher">Cacher for result.</param>
        /// <param name="workCacher">Cacher for work.</param>
        /// <param name="interval">Interval of cruiser (ms).</param>
        /// <param name="concurrency">Max count of concurrency works.</param>
        /// <param name="resolver">Resolver to check retrieable.</param>
        public AsyncWorkMonoHub(ICacher<object> resultCacher = null, ICacher<IAsyncWork> workCacher = null,
            int interval = 250, int concurrency = 3, IRetryResolver resolver = null) :
            base(resultCacher, workCacher, interval, concurrency, resolver)
        {
            Instruction = new WaitForEndOfFrame();
        }

        /// <summary>
        /// Activate cruiser.
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            if (notifier == null)
            {
                notifier = new GameObject(nameof(WorkHubBehaviour)).AddComponent<WorkHubBehaviour>();
                Object.DontDestroyOnLoad(notifier.gameObject);
                notifier.StartCoroutine(StartNotifier());
            }
        }

        /// <summary>
        /// Deactivate cruiser.
        /// </summary>
        public override void Deactivate()
        {
            base.Deactivate();
            if (notifier != null)
            {
                Object.Destroy(notifier.gameObject);
                notifier = null;
            }
        }

        /// <summary>
        /// Start notifier to tick loop.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator StartNotifier()
        {
            while (true)
            {
                NotifyStatus();
                yield return Instruction;
            }
        }
    }
}