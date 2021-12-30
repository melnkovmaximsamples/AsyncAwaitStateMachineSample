using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using StateMachine.Console;

// Обычный void
var @voidTests = new StateMachineVoidTests();

@voidTests.DoSomethingBefore(() => Task.Delay(0));
@voidTests.DoSomethingAfter(() => Task.Delay(0));

// Task<int>, с другими типами по такому же принципу работает...
var @taskTests = new StateMachineTaskTests();
Func<Task<int>> secondAction = async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(3));
    return 25;
};
var thirdAction = new Func<int>(() => 50);

Console.WriteLine("--- Before ---");
var resultrBefore = await @taskTests.DoSomethingBefore(Task.Delay(TimeSpan.FromSeconds(3)), Task.Run(secondAction), thirdAction);
Console.WriteLine($"Result {resultrBefore}");

Console.WriteLine("--- After ---");
var resultAfter =  await @taskTests.DoSomethingAfter(Task.Delay(TimeSpan.FromSeconds(3)), Task.Run(secondAction), thirdAction);
Console.WriteLine($"Result {resultAfter}");

Console.WriteLine("Done");
Console.ReadKey();

/*
async void TestVoidBefore()
{
    var result = await TestInteger();
    
    Console.WriteLine(result);
}

async void TestVoidAfeter()
{
    
}

void TestVoid()
{
    var stateMachine = new StateMachineByVoidBuilder()
    {
        Builder = new AsyncVoidMethodBuilder()
    };

    stateMachine.Builder.Start(ref stateMachine);
}

Task<int> TestInteger()
{
    var stateMachine = new StateMachineByTaskBuilderWithManySteps()
    {
        Builder = new AsyncTaskMethodBuilder<int>()
    };

    stateMachine.Builder.Start(ref stateMachine);

    return stateMachine.Builder.Task;
}
*/