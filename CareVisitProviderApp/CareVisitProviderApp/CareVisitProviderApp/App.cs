using System;

namespace CareVisitProviderApp
{
    public class App
    {
        public static void Initialize()
        {
            ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
        }
    }
}
