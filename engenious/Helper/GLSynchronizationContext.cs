using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace engenious
{
    sealed class GLSynchronizationContext : SynchronizationContext
    {
        readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue =
            new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();

        public override void Post(SendOrPostCallback d, object state)
        {
            _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
        }

        public void RunOnCurrentThread()
        {
            KeyValuePair<SendOrPostCallback, object> workItem;

            while (_queue.Count > 0 && _queue.TryTake(out workItem, Timeout.Infinite))
                workItem.Key(workItem.Value);
        }
    }
}