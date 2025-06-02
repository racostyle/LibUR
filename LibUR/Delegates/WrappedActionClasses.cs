using System;

namespace LibUR.Delegates
{
    public class WrappedAction
    {
        private Action _action;

        public void Register(Action action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke()
        {
            _action?.Invoke();
        }
    }

    public class WrappedAction<T1>
    {
        private Action<T1> _action;

        public void Register(Action<T1> action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke(T1 arg1)
        {
            _action?.Invoke(arg1);
        }
    }

    public class WrappedAction<T1, T2>
    {
        private Action<T1, T2> _action;

        public void Register(Action<T1, T2> action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            _action?.Invoke(arg1, arg2);
        }
    }

    public class WrappedAction<T1, T2, T3>
    {
        private Action<T1, T2, T3> _action;

        public void Register(Action<T1, T2, T3> action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            _action?.Invoke(arg1, arg2, arg3);
        }
    }

    public class WrappedAction<T1, T2, T3, T4>
    {
        private Action<T1, T2, T3, T4> _action;

        public void Register(Action<T1, T2, T3, T4> action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _action?.Invoke(arg1, arg2, arg3, arg4);
        }
    }

    public class WrappedAction<T1, T2, T3, T4, T5>
    {
        private Action<T1, T2, T3, T4, T5> _action;

        public void Register(Action<T1, T2, T3, T4, T5> action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            _action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
    }

    public class WrappedAction<T1, T2, T3, T4, T5, T6>
    {
        private Action<T1, T2, T3, T4, T5, T6> _action;

        public void Register(Action<T1, T2, T3, T4, T5, T6> action)
        {
            if (_action == null)
                _action = action;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            _action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
}
