namespace LibUR.Auxiliary
{
    /// <summary>Returns true from CanUpdate() every N calls (e.g. every N frames). 0 or 1 = every call; 2 = every 2nd; 3 = every 3rd; etc.</summary>
    public class FrameThrottle
    {
        private readonly int LIMIT;
        private int _counter;

        /// <param name="limit">0 or 1 = every call (always true). 2 = every 2nd call, 3 = every 3rd, and so on.</param>
        /// <param name="randomStartIndex">If true, first "true" can occur on any of the first limit calls. Ignored when limit is 0 or 1.</param>
        public FrameThrottle(int limit, bool randomStartIndex = true)
        {
            LIMIT = limit;
            if (limit <= 1)
                _counter = 0;
            else if (randomStartIndex)
                _counter = UnityEngine.Random.Range(0, limit);
            else
                _counter = limit;
        }

        public bool CanUpdate()
        {
            if (LIMIT <= 1)
                return true;
                
            _counter = _counter < LIMIT ? ++_counter : 1;
            return _counter == LIMIT;
        }
    }
}