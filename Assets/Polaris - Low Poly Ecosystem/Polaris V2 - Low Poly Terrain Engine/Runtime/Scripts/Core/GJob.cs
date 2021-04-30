using System;

namespace Pinwheel.Griffin
{
    internal class GJob
    {
        internal int Order { get; set; }
        internal Action Action { get; set; }

        internal void Run()
        {
            if (Action != null)
            {
                Action.Invoke();
            }
        }
    }
}
