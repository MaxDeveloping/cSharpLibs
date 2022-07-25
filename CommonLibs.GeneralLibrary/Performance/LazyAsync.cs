using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibs.GeneralLibrary.Performance
{
    public class LazyAsync<T>
    {
        private readonly Lazy<Task<T>> m_Instance;

        public LazyAsync(Func<T> pFactoryFunc)
        {
            m_Instance = new Lazy<Task<T>>(() => Task.Run(pFactoryFunc));
        }

        public LazyAsync(Func<Task<T>> pFactoryFunc)
        {
            m_Instance = new Lazy<Task<T>>(() => Task.Run(pFactoryFunc));
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return m_Instance.Value.GetAwaiter();
        }
    }
}
