using System;

namespace LibUR.Signals
{
    public struct SubscriberData
    {
        internal string Name;
        internal Action Action;

        public SubscriberData(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
