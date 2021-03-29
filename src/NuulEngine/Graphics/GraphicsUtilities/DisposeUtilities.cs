using System;
using System.Collections.Generic;

namespace NuulEngine.Graphics.GraphicsUtilities
{
    internal static class DisposeUtilities
    {
        public static void DisposeDictionaryElements<T>(Dictionary<string, T> dictionary)
            where T : class, IDisposable
        {
            foreach (var value in dictionary.Values)
            {
                value.Dispose();
            }

            dictionary.Clear();
        }

        public static void DisposeListElements<T>(List<T> list)
            where T : class, IDisposable
        {
            foreach (var element in list)
            {
                element.Dispose();
            }

            list.Clear();
        }
    }
}
