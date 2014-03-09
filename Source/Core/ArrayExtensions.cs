using System;

namespace SqlFramework
{
    public static class ArrayExtensions
    {
        public static TItem[] ForEach<TItem>(this TItem[] items, Action<TItem> action)
        {
            foreach (var item in items)
            {
                action(item);
            }

            return items;
        }
    }
}