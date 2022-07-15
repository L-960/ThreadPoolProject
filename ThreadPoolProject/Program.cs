// See https://aka.ms/new-console-template for more information

using ThreadPoolProject.ThreadPool;

Console.WriteLine("Hello, World!");

FixedThreadPool threadPool = FixedThreadPool.Create(4);
for (int i = 0; i < 8; i++)
{
    threadPool.Execute(() =>
    {
        Thread.Sleep(1000 * 2);
        Console.WriteLine($"ManagedThreadId:{Thread.CurrentThread.ManagedThreadId} 干完活了");
    });
}

string? sign = Console.ReadLine();

if (sign == "1")
{
    threadPool.Shutdown();
}

Console.ReadLine();