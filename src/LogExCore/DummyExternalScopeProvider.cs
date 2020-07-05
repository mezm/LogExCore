using Microsoft.Extensions.Logging;
using System;

namespace LogExCore
{
    internal class DummyExternalScopeProvider : IExternalScopeProvider
    {
        private DummyExternalScopeProvider()
        {
        }

        public static DummyExternalScopeProvider Instance { get; } = new DummyExternalScopeProvider();

        public void ForEachScope<TState>(Action<object, TState> callback, TState state)
        {
        }

        public IDisposable Push(object state) => DummyScope.Instance;

        private class DummyScope : IDisposable
        {
            public static DummyScope Instance { get; } = new DummyScope();

            public void Dispose()
            {
            }
        }
    }
}
