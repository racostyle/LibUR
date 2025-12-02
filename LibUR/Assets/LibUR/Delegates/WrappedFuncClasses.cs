using System;

namespace LibUR.Delegates
{
    public class WrappedFunc<TX>
    {
        private Func<TX> _func;

        public void Register(Func<TX> func)
        {
            if (_func == null)
                _func = func;
        }

        public TX Invoke()
        {
            return _func.Invoke();
        }
    }

    public class WrappedFunc<T1, TX>
    {
        private Func<T1, TX> _func;

        public void Register(Func<T1, TX> func)
        {
            if (_func == null)
                _func = func;
        }

        public TX Invoke(T1 arg1)
        {
            return _func.Invoke(arg1);
        }
    }

    public class WrappedFunc<T1, T2, TX>
    {
        private Func<T1, T2, TX> _func;

        public void Register(Func<T1, T2, TX> func)
        {
            if (_func == null)
                _func = func;
        }

        public TX Invoke(T1 arg1, T2 arg2)
        {
            return _func.Invoke(arg1, arg2);
        }
    }

    public class WrappedFunc<T1, T2, T3, TX>
    {
        private Func<T1, T2, T3, TX> _func;

        public void Register(Func<T1, T2, T3, TX> func)
        {
            if (_func == null)
                _func = func;
        }

        public TX Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            return _func.Invoke(arg1, arg2, arg3);
        }
    }

    public class WrappedFunc<T1, T2, T3, T4, TX>
    {
        private Func<T1, T2, T3, T4, TX> _func;

        public void Register(Func<T1, T2, T3, T4, TX> func)
        {
            if (_func == null)
                _func = func;
        }

        public TX Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return _func.Invoke(arg1, arg2, arg3, arg4);
        }
    }

    public class WrappedFunc<T1, T2, T3, T4, T5, TX>
    {
        private Func<T1, T2, T3, T4, T5, TX> _func;

        public void Register(Func<T1, T2, T3, T4, T5, TX> func)
        {
            if (_func == null)
                _func = func;
        }

        public TX Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return _func.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
    }
}
