namespace mFramework.Storage
{
    public class mStorage : IKeyValueStorage
    {
        public static IKeyValueStorage Instance { get; } = new mStorage();
    }
}
