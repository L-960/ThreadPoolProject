namespace ConsoleApp1.ThreadPool
{

    public interface IThreadStarter
    {
        // 创建并启动一个线程
        Thread StartThread(Action action);

        // 设置线程名
        void setThreadName(string? name);
    }
}
