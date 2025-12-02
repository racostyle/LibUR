namespace LibUR.Instantiation
{
    public interface IInitializable
    {
        void Initialize(params object[] args);
        void Terminate();
    }
}



