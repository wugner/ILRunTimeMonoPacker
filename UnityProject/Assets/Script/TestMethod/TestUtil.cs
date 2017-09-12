using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class TestUtil
{
    public static void ILSort<R>(this List<R> list, Func<object, object, int> predicate)
    {
        list.Sort((p1, p2) =>
        {
            return (predicate(p1, p2));
        });
    }
}