using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    internal static class TaskEx
    {
        public static TaskAwaiter GetAwaiter(this Task task)
        {
            return new TaskAwaiter(task);
        }

        public static TaskAwaiter<T> GetAwaiter<T>(this Task<T> task)
        {
            return new TaskAwaiter<T>(task);
        }
        public static Task Run(Action action)
        {
            return Task.Factory.StartNew(action);
        }
    }
    internal struct TaskAwaiter : INotifyCompletion
    {
        readonly Task task;
        internal TaskAwaiter(Task task)
        {
            this.task = task;
        }

        internal static TaskScheduler TaskScheduler
        {
            get
            {
                if (SynchronizationContext.Current == null)
                    return TaskScheduler.Default;
                else
                    return TaskScheduler.FromCurrentSynchronizationContext();
            }
        }

        public bool IsCompleted
        {
            get { return task.IsCompleted; }
        }

        public void OnCompleted(Action continuation)
        {
            this.task.ContinueWith(
                delegate (Task task) {

                    //Console.WriteLine("状态机回调函数信息："+Thread.CurrentThread.ManagedThreadId);
                    //Console.WriteLine("\t名称："+continuation.Method.Name);
                    //Console.WriteLine("\t所属类型：" + continuation.Method.DeclaringType.ToString());
                    continuation();
                }, TaskAwaiter.TaskScheduler);
        }

        public void GetResult()
        {
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                throw ex;
            }
        }
    }

    internal struct TaskAwaiter<T> : INotifyCompletion
    {
        readonly Task<T> task;

        internal TaskAwaiter(Task<T> task)
        {
            this.task = task;
        }

        public bool IsCompleted
        {
            get { return task.IsCompleted; }
        }

        public void OnCompleted(Action continuation)
        {
            this.task.ContinueWith(
                delegate (Task<T> task) {
                    continuation();
                }, TaskAwaiter.TaskScheduler);
        }

        public T GetResult()
        {
            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                throw ex;
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    internal interface INotifyCompletion
    {
        void OnCompleted(Action continuation);
    }

    internal interface ICriticalNotifyCompletion : INotifyCompletion
    {
        [SecurityCritical]
        void UnsafeOnCompleted(Action continuation);
    }

    internal interface IAsyncStateMachine
    {
        void MoveNext();
        void SetStateMachine(IAsyncStateMachine stateMachine);
    }

    internal struct AsyncVoidMethodBuilder
    {
        public static AsyncVoidMethodBuilder Create()
        {
            return new AsyncVoidMethodBuilder();
        }

        public void SetException(Exception exception)
        {
            throw exception;
        }

        public void SetResult()
        {
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // Should not get called as we don't implement the optimization that this method is used for.
            throw new NotImplementedException();
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
    }

    internal struct AsyncTaskMethodBuilder
    {
        TaskCompletionSource<object> tcs;

        public Task Task { get { return tcs.Task; } }

        public static AsyncTaskMethodBuilder Create()
        {
            AsyncTaskMethodBuilder b;
            b.tcs = new TaskCompletionSource<object>();
            return b;
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // Should not get called as we don't implement the optimization that this method is used for.
            throw new NotImplementedException();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void SetResult()
        {
            tcs.SetResult(null);
        }

        public void SetException(Exception exception)
        {
            tcs.SetException(exception);
        }
    }

    internal struct AsyncTaskMethodBuilder<T>
    {
        TaskCompletionSource<T> tcs;

        public Task<T> Task { get { return tcs.Task; } }

        public static AsyncTaskMethodBuilder<T> Create()
        {
            AsyncTaskMethodBuilder<T> b;
            b.tcs = new TaskCompletionSource<T>();
            return b;
        }

        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            // Should not get called as we don't implement the optimization that this method is used for.
            throw new NotImplementedException();
        }

        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        public void SetResult(T result)
        {
            tcs.SetResult(result);
        }

        public void SetException(Exception exception)
        {
            tcs.SetException(exception);
        }
    }
}