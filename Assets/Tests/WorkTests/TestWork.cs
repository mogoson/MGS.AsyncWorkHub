using System;
using System.Threading;
using UnityEngine;

namespace MGS.Work.Tests
{
    public class TestWork : AsyncWork<string>
    {
        public TestWork(string key = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                Key = key;
            }
        }

        protected override string OnExecute()
        {
            Thread.Sleep(1000);
            Progress += 0.25f;
            Debug.Log($"Work {Key} OnExecute Progress {Progress}");

            Thread.Sleep(1000);
            Progress += 0.25f;
            Debug.Log($"Work {Key} OnExecute Progress {Progress}");

            Thread.Sleep(1000);
            Progress += 0.25f;
            Debug.Log($"Work {Key} OnExecute Progress {Progress}");

            Thread.Sleep(1000);
            Progress += 0.25f;
            Debug.Log($"Work {Key} OnExecute Progress {Progress}");
            return "Result of TestWork";
        }
    }

    public class TestErrorWork : AsyncWork<string>
    {
        protected override string OnExecute()
        {
            Thread.Sleep(1000);
            Progress += 0.25f;
            Debug.Log($"Work {Key} OnExecute Progress {Progress}");

            throw new Exception("We throw an exception to test error.");
        }
    }
}