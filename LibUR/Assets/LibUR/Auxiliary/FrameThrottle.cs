namespace LibUR.Auxiliary
{
    public class FrameThrottle
    {
        private readonly int LIMIT;
        private int _counter;

        public FrameThrottle(int limit, bool randomStartIndex = true)
        {
            if (randomStartIndex)
                _counter = UnityEngine.Random.Range(0, limit);
            else
                _counter = 0;

            LIMIT = limit;
        }

        public bool CanUpdate()
        {
            _counter = _counter < LIMIT ? ++_counter : 0;

            return _counter == LIMIT;
        }
    }
}