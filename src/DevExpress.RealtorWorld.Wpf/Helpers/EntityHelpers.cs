using System;
using System.Linq;

namespace DevExpress.RealtorWorld.Xpf.Helpers {
    public static class QuerableExtensions {
        public static void Load<T>(this IQueryable<T> q) { }
    }
}
