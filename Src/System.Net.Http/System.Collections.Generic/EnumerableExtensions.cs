using System.Linq;

namespace System.Collections.Generic {
	public static class EnumerableExtensions {
		public static T Find<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) {
			return enumerable.FirstOrDefault(t => predicate(t));
		}
	}
}
