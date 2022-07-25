using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommonLibs.GeneralLibrary.Extensions
{
    public static class CollectionEx
    {
        public static void AddRange<T>(this ICollection<T> pCollection, IEnumerable<T> pItems)
        {
            foreach (var item in pItems)
                pCollection.Add(item);
        }
    }
}
