using System;
using System.Collections.Generic;

namespace LibUnity.Frontend.Extensions
{
    public static class DisposableExtensions
    {
        public static void DisposeAllAndClear(this IList<IDisposable> disposables)
        {
            if (disposables is null)
            {
                return;
            }

            for (var i = disposables.Count; i > 0; i--)
            {
                disposables[i - 1].Dispose();
            }

            disposables.Clear();
        }
    }
}
