using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace StateMachine.Console
{
    public struct StateMachineByTaskBuilder : IAsyncStateMachine
    {
        public AsyncTaskMethodBuilder<int> Builder;
        public int State;

        /// <summary>
        /// Только для того, чтобы дать возможность работать с любыми делегатами
        /// По умолчанию всё генерится в MoveNext
        /// </summary>

        public Task FirstAction;

        public Task<int> SecondAction;
        public Func<int> ThirdAction;

        private TaskAwaiter _firstAwaiter;
        private TaskAwaiter<int> _secondAwaiter;
        
        private int _secondActionResult;
        private int _thirdActionResult;

        public void MoveNext()
        {
            System.Console.WriteLine($"{nameof(StateMachineByTaskBuilder)} doing move next");
            
            _firstAwaiter = FirstAction.GetAwaiter();
            _secondAwaiter = SecondAction.GetAwaiter();

            try
            {
                switch (State)
                {
                    case 0:
                        System.Console.WriteLine($"Enter to case 0");
                        if (_firstAwaiter.IsCompleted)
                        {
                            State = 1;
                            goto case 1;
                        }

                        System.Console.WriteLine($"Getting result from {nameof(_firstAwaiter)}");
                        _firstAwaiter.GetResult();
                        
                        Builder.AwaitUnsafeOnCompleted(ref _firstAwaiter, ref this);
                        return;
                    case 1:
                        System.Console.WriteLine($"Enter to case 1");
                        if (_secondAwaiter.IsCompleted)
                        {
                            State = 2;
                            _secondActionResult = _secondAwaiter.GetResult();
                            goto case 2;
                        }
                        
                        System.Console.WriteLine($"Getting result from {nameof(_secondAwaiter)}");
                        _secondActionResult = _secondAwaiter.GetResult();
                        
                        System.Console.WriteLine(_secondActionResult);
                        Builder.AwaitUnsafeOnCompleted(ref _secondAwaiter, ref this);
                        
                        return;
                    case 2:
                        System.Console.WriteLine($"Enter to case 2");
                        _thirdActionResult = ThirdAction();
                        break;
                }
            }
            catch (Exception ex)
            {
                Builder.SetException(ex);
            }
            
            Builder.SetResult(_secondActionResult + _thirdActionResult);
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            
        }
    }

    public class StateMachineTaskTests
    {
        public async Task<int> DoSomethingBefore(Task firstAction, Task<int> secondAction, Func<int> thirdAction)
        {
            await firstAction;

            var secondActionResult = await secondAction;
            var thirdActionResult = thirdAction();

            return secondActionResult + thirdActionResult;
        }

        public Task<int> DoSomethingAfter(Task firstAction, Task<int> secondAction, Func<int> thirdAction)
        {
            var stateMachine = new StateMachineByTaskBuilder()
            {
                Builder = new AsyncTaskMethodBuilder<int>(),
                FirstAction = firstAction,
                SecondAction = secondAction,
                ThirdAction = thirdAction
            };
            
            stateMachine.Builder.Start(ref stateMachine);

            return stateMachine.Builder.Task;
        }
    }
}