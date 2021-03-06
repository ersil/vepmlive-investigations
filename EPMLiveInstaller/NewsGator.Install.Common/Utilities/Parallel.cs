﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace NewsGator.Install.Common.Utilities
{
    internal static class Parallel
    {
        private static int NumberOfParallelTasks = 1;
        private static int WorkerThreads = 1;
        private static int CompletionPortThreads = 1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Parallel()
        {
            ThreadPool.GetAvailableThreads(out WorkerThreads, out CompletionPortThreads);
            NumberOfParallelTasks = WorkerThreads / 2;
        }

        internal static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            var syncRoot = new object();

            if (enumerable == null) return;

            var enumerator = enumerable.GetEnumerator();

            InvokeAsync<T> del = InvokeAction;

            var seedItemArray = new T[NumberOfParallelTasks];
            var resultList = new List<IAsyncResult>(NumberOfParallelTasks);

            for (int i = 0; i < NumberOfParallelTasks; i++)
            {
                bool moveNext;

                lock (syncRoot)
                {
                    moveNext = enumerator.MoveNext();
                    seedItemArray[i] = enumerator.Current;
                }

                if (moveNext)
                {
                    var iAsyncResult = del.BeginInvoke
             (enumerator, action, seedItemArray[i], syncRoot, i, null, null);
                    resultList.Add(iAsyncResult);
                }
            }

            foreach (var iAsyncResult in resultList)
            {
                del.EndInvoke(iAsyncResult);
                iAsyncResult.AsyncWaitHandle.Close();
            }
        }

        delegate void InvokeAsync<T>(IEnumerator<T> enumerator,
        Action<T> achtion, T item, object syncRoot, int i);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        static void InvokeAction<T>(IEnumerator<T> enumerator, Action<T> action,
                T item, object syncRoot, int i)
        {
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name =
            String.Format("Parallel.ForEach Worker Thread No:{0}", i);

            bool moveNext = true;

            while (moveNext)
            {
                action.Invoke(item);

                lock (syncRoot)
                {
                    moveNext = enumerator.MoveNext();
                    item = enumerator.Current;
                }
            }
        }
    } 
}
