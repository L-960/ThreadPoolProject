using System.Collections.Concurrent;

namespace ConsoleApp1.ThreadPool;

public abstract class IThreadPool<T>
{
    // 核心线程数
    protected int CorePoolSize;

    // 最大线程数
    protected int? MaximumPoolSize;

    // 存活时间
    protected int? KeepAliveTime;
    
    // 任务队列，初始化
    protected Queue<T> WorkQueue;
    
    protected List<Thread>? Workers;
    
    protected IThreadStarter ThreadStarter;
    
    // 线程池初始化
    protected IThreadPool(IThreadStarter threadStarter,Queue<T> workQueue,int? maximumPoolSize, int corePoolSize,int? keepAliveTime)
    {
        InitCommonPool(threadStarter,workQueue,maximumPoolSize, corePoolSize,keepAliveTime);
    }

    protected IThreadPool(IThreadStarter threadStarter,Queue<T> workQueue, int corePoolSize)
    {
        InitCommonPool(threadStarter,workQueue,null, corePoolSize,null);
    }

    private void InitCommonPool(IThreadStarter threadStarter,Queue<T> workQueue,int? maximumPoolSize, int corePoolSize,int? keepAliveTime)
    {
        this.ThreadStarter = threadStarter;
        this.WorkQueue = workQueue;
        this.MaximumPoolSize = maximumPoolSize;
        this.CorePoolSize = corePoolSize;
        this.KeepAliveTime = keepAliveTime;
        this.Workers = new List<Thread>();
        for (int i = 0; i < corePoolSize; i++)
        {
            Thread thread = this.ThreadStarter.StartThread(Excutor);
            Console.WriteLine($"创建了线程 {thread.ManagedThreadId}");
            Workers.Add(thread);
        }
    }
    
    
    // 任务执行器
    protected virtual void Excutor()
    {
    }

    // 丢弃任务，关闭线程池
    public virtual void ShutdownNow()
    {
    }

    // 等待任务完成后，关闭线程池
    public virtual void Shutdown()
    {
    }

    // 接收任务(任务队列满，则阻塞、抛异常、或...)
    public virtual void Execute(Action command)
    {
    }
}