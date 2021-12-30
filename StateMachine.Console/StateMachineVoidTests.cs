using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StateMachine.Console
{
    public struct StateMachineByVoidBuilder: IAsyncStateMachine
    {
        public AsyncVoidMethodBuilder Builder;
        
        /// <summary>
        /// Только для того, чтобы дать возможность работать с любыми делегатами
        /// По умолчанию, таска генерится сразу в методе MoveNext
        /// </summary>
        public Task task;
        
        public void MoveNext()
        {
            var awaiter = task.GetAwaiter();

            if (awaiter.IsCompleted) return;

            try
            {
                awaiter.GetResult();
            }
            catch (Exception ex)
            {
                Builder.SetException(ex);
            }
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            
        }
    }

    public class StateMachineVoidTests
    {
        public async void DoSomethingBefore(Action action)
        {
            await Task.Run(action);
        }

        public void DoSomethingAfter([NotNull] Action action)
        {
            var stateMachine = new StateMachineByVoidBuilder()
            {
                Builder = new AsyncVoidMethodBuilder(),
                task = Task.Run(action)
            };
            
            stateMachine.Builder.Start(ref stateMachine);
        }
    }
}