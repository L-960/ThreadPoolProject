namespace ConsoleApp1.ThreadPool
{
    public class FixThreadStarter : IThreadStarter
    {
        private string? _threadName = "Fix-Thread-";

        public FixThreadStarter()
        {
        }
        
        public FixThreadStarter(string? threadName)
        {
            this._threadName = threadName;
        }
        
        public Thread StartThread(Action action)
        {
            Thread t = new Thread(() => action())
            {
                IsBackground = true,
                Name = _threadName
            };
            t.Start();
            return t;
        }

        public void setThreadName(string? name)
        {
            _threadName = name;
        }
    }
}