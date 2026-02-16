using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Assets.LibUR.Auxiliary
{
    public static class SessionObjectId
    {
        private sealed class Box { public readonly int Id; public Box(int id) => Id = id; }

        private static int _nextId = 0;
        private static readonly ConditionalWeakTable<object, Box> _ids = new();

        /// <summary>Returns a unique ID for this object for the lifetime of the current play session.</summary>
        public static int Get(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            return _ids.GetValue(obj, _ => new Box(Interlocked.Increment(ref _nextId))).Id;
        }

        /// <summary>Optional manual reset (eg if you disable domain reload and want a clean session).</summary>
        public static void ResetForNewSession()
        {
            _nextId = 0;
            // ConditionalWeakTable cannot be cleared; reinitialize by recreating the app domain normally.
            throw new NotSupportedException("Use the Dictionary-based variant if you need manual reset without domain reload.");
        }
    }
}
