using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MGS.Work.Tests
{
    public class AsyncWorkTest
    {
        [Test]
        public void ExecuteTest()
        {
            var work = new TestWork();
            work.Execute();

            Assert.IsNull(work.Error);
            Debug.Log($"work.Result {work.Result}");
            Assert.IsNotNull(work.Result);
        }

        [Test]
        public void ExecuteErrorTest()
        {
            var work = new TestErrorWork();
            work.Execute();

            Assert.IsNull(work.Result);
            Assert.IsNotNull(work.Error);
            Debug.Log($"work.Error {work.Error.Message}");
        }

        [UnityTest]
        public IEnumerator ExecuteAsyncTest()
        {
            var work = new TestWork();
            work.ExecuteAsync();

            while (!work.IsDone)
            {
                yield return null;
                Debug.Log($"Speed {work.Speed} byte/s");
                Debug.Log($"Progress {work.Progress}");
            }

            Assert.IsNull(work.Error);
            Debug.Log($"work.Result {work.Result}");
            Assert.IsNotNull(work.Result);
        }
    }
}