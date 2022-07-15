using System.Collections.Concurrent;

namespace ThreadPoolProject.ThreadPool;

public class FixedThreadPool : IThreadPool<Action>
{
    // 任务队列锁
    private readonly object _lockerQ = new object();
    
    // 丢弃任务关机锁
    private readonly object _lockShutdownNow = new object();
    
    // 完成任务后关机锁
    private readonly object _lockShutdown = new object();

    private bool _isShutdown;
    private bool _isShutdownNow;

    // 阻塞信号集合
    private readonly BlockingCollection<AutoResetEvent> _eventQueue = new();

    public static FixedThreadPool Create(int corePoolSize)
    {
        IThreadStarter newThreadStarter = new FixThreadStarter();
        Queue<Action> wordQueue = new Queue<Action>();
        return new FixedThreadPool(newThreadStarter, wordQueue, corePoolSize);
    }

    private FixedThreadPool(IThreadStarter threadStarter, Queue<Action> workQueue, int corePoolSize) : base(
        threadStarter, workQueue, corePoolSize)
    {
    }

    protected override void Excutor()
    {
        AutoResetEvent eEvent = new AutoResetEvent(false);
        while (true)
        {
            // 重置信号 通知我已经空闲了
            _eventQueue.Add(eEvent);
            
            // 等待执行信号
            Console.WriteLine($"{DateTime.Now} 线程 ThreadId:{Thread.CurrentThread.ManagedThreadId} 开始等待任务执行信号...");
            eEvent.WaitOne();
            Console.WriteLine($"{DateTime.Now} 线程 ThreadId:{Thread.CurrentThread.ManagedThreadId} 接收到任务执行信号，开始执行");

            lock (_lockShutdown)
            {
                if (_isShutdown)
                {
                    lock (_lockerQ)
                    {
                        if (WorkQueue.Count <= 0)
                        {
                            Console.WriteLine(
                                $"剩余任务数：{WorkQueue.Count} ThreadId:{Thread.CurrentThread.ManagedThreadId} 完成任务后 退出了");
                            break;
                        }
                    }
                }
            }

            lock (_lockShutdownNow)
            {
                if (_isShutdownNow)
                {
                    lock (_lockerQ)
                    {
                        Console.WriteLine(
                            $"剩余任务数：{WorkQueue.Count} ThreadId:{Thread.CurrentThread.ManagedThreadId} 强制退出了");
                    }

                    break;
                }
            }

            // 执行任务
            Action command;
            lock (_lockerQ)
            {
                command = WorkQueue.Dequeue();
            }

            command.Invoke();
        }
    }

    // 丢弃任务，关闭线程池
    public override void ShutdownNow()
    {
        lock (_lockShutdownNow)
        {
            _isShutdownNow = true;
        }

        // 通知线程关闭
        for (int i = 0; i < CorePoolSize; i++)
        {
            AutoResetEvent @event = _eventQueue.Take();
            @event.Set();
        }

        Console.WriteLine("强制关机");
    }

    //等待任务完成后，关闭线程池
    public override void Shutdown()
    {
        lock (_lockShutdown)
        {
            _isShutdown = true;
        }

        // 通知线程关闭
        for (int i = 0; i < CorePoolSize; i++)
        {
            AutoResetEvent @event = _eventQueue.Take();
            @event.Set();
        }

        Console.WriteLine("任务已完成，关机");
    }

    // 接收任务(任务队列满，则阻塞、抛异常、或...)
    public override void Execute(Action command)
    {
        if (command == null)
            throw new NullReferenceException();

        // 放置任务
        lock (_lockerQ)
        {
            WorkQueue.Enqueue(command);
        }

        // 随机通知一个线程去执行(),如果没有空闲线程，则等待
        AutoResetEvent eEvent = _eventQueue.Take();
        eEvent.Set();
    }
}