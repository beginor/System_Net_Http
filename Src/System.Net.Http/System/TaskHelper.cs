using System.Threading.Tasks;

namespace System {
	public static class TaskHelper {
		public static Task<T> FromResult<T>(T result) {
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(result);
			return tcs.Task;
		}
	}
}
