using System;
using System.Collections;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MGS.Work.Tests
{
    public class AsyncWorkStatusHubTest
    {
        IAsyncWorkStatusHub hub;
        IAsyncWorkHandler<string> handler;
        Thread thread;

        [SetUp]
        public void SetUp()
        {
            hub = WorkHubFactory.CreateStatusHub();
            hub.Activate();

            thread = new Thread(() =>
            {
                while (true)
                {
                    hub.NotifyStatus();
                    Thread.Sleep(100);
                }
            })
            { IsBackground = true };
            thread.Start();
        }

        [TearDown]
        public void TearDown()
        {
            thread.Abort();
            thread = null;

            handler.Abort();
            handler = null;

            hub.Deactivate();
            hub = null;
        }

        [UnityTest]
        public IEnumerator CallbackTest()
        {
            var progress = 0f;
            string result = null;
            Exception error = null;

            handler = hub.Enqueue(new TestWork());
            handler.OnProgressChanged += p =>
            {
                progress = p;
                Debug.Log($"Progress {p}");
            };
            handler.OnSpeedChanged += speed =>
            {
                Debug.Log($"Speed {speed} byte/s");
            };
            handler.OnCompleted += (r, e) =>
            {
                result = r;
                error = e;

                if (e == null)
                {
                    Debug.Log($"Result {result}");
                }
                else
                {
                    Debug.Log($"Error {error.Message}/{error.StackTrace}");
                }
            };

            yield return handler.WaitDone();
            yield return new WaitForSeconds(1.0f);

            Assert.IsNull(error);
            Debug.Log($"work.progress {progress}");
            Assert.IsTrue(progress > 0);

            Debug.Log($"work.Result {result}");
            Assert.IsNotNull(result);
        }
    }
}