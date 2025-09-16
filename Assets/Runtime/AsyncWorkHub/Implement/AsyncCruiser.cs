/*************************************************************************
 *  Copyright © 2025 Mogoson All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  IAsyncCruiser.cs
 *  Description  :  Default.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0.0
 *  Date         :  09/16/2025
 *  Description  :  Initial development version.
 *************************************************************************/

using System.Threading;

namespace MGS.Work
{
    public abstract class AsyncCruiser : IAsyncCruiser
    {
        /// <summary>
        /// Cruiser is active?
        /// </summary>
        public bool IsActive { get { return thread != null && thread.IsAlive; } }

        /// <summary>
        /// Interval of cruiser (ms).
        /// </summary>
        public int Interval { set; get; }

        /// <summary>
        /// Cruiser thread
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="interval">Interval of cruiser (ms).</param>
        public AsyncCruiser(int interval = 250)
        {
            Interval = interval;
        }

        /// <summary>
        /// Activate cruiser.
        /// </summary>
        public virtual void Activate()
        {
            if (thread == null)
            {
                thread = new Thread(StartCruiser) { IsBackground = true };
                thread.Start();
            }
        }

        /// <summary>
        /// Deactivate cruiser.
        /// </summary>
        public virtual void Deactivate()
        {
            if (thread == null)
            {
                return;
            }

            if (thread.IsAlive)
            {
                thread.Abort();
            }
            thread = null;
        }

        /// <summary>
        /// Start cruiser to tick loop.
        /// </summary>
        protected void StartCruiser()
        {
            while (true)
            {
                CruiserTick();
                Thread.Sleep(Interval);
            }
        }

        /// <summary>
        /// Cruiser tick every cycle.
        /// </summary>
        protected abstract void CruiserTick();
    }
}