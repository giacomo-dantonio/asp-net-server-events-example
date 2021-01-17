using System;
using System.Threading;

namespace CounterApi.Models
{
    public class PersistentCounter : IDisposable
    {
        private readonly object locker = new object();

        private Timer _timer;

        public int Value { get; set; }

        public bool Running { get; set; } = true;

        public event EventHandler<int> CounterUpdated;

        public PersistentCounter()
        {
            _timer = new Timer(_ => {
                if (Running)
                {
                    Value++;
                    CounterUpdated?.Invoke(this, Value);
                }
            });
            _timer.Change(0, 1000);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}