using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MGS.Work.Tests
{
    public class AsyncWorkHubTest
    {
        IAsyncWorkHub hub;
        IAsyncWork<string> work;

        [SetUp]
        public void SetUp()
        {
            hub = WorkHubFactory.CreateHub(250, 3);
            hub.Activate();
        }

        [TearDown]
        public void TearDown()
        {
            work.AbortAsync();
            work = null;

            hub.Deactivate();
            hub = null;
        }

        [UnityTest]
        public IEnumerator EnqueueWorkTest()
        {
            work = hub.Enqueue(new TestWork());
            while (!work.IsDone)
            {
                yield return null;
                Debug.Log($"Progress {work.Progress}");
            }

            Assert.IsNull(work.Error);
            Debug.Log($"work.Result {work.Result}");
            Assert.IsNotNull(work.Result);
        }

        [UnityTest]
        public IEnumerator ConcurrencyTest()
        {
            work = hub.Enqueue(new TestWork());
            work = hub.Enqueue(new TestWork());
            work = hub.Enqueue(new TestWork());
            work = hub.Enqueue(new TestWork());

            yield return new WaitForSeconds(3.0f);

            Debug.Log($"hub.Waitings {hub.Waitings}");
            Debug.Log($"hub.Workings {hub.Workings}");

            Assert.AreEqual(1, hub.Waitings);
            Assert.AreEqual(3, hub.Workings);
        }

        [UnityTest]
        public IEnumerator ClearTest()
        {
            work = hub.Enqueue(new TestWork());
            work = hub.Enqueue(new TestWork());
            work = hub.Enqueue(new TestWork());

            yield return new WaitForSeconds(3.0f);
            hub.Clear(true, true);

            Debug.Log($"hub.Waitings {hub.Waitings}");
            Debug.Log($"hub.Workings {hub.Workings}");

            Assert.AreEqual(0, hub.Waitings);
            Assert.AreEqual(0, hub.Workings);
        }
    }
}