using System.Security;

namespace System.Net.Http {

	public static class StringExtensions {
		
		public static bool IsNullOrEmpty(this string str) {
			return string.IsNullOrEmpty(str);
		}

		public static bool IsNotNullOrEmpty(this string str) {
			return !str.IsNullOrEmpty();
		}

		public static bool IsNullOrWhiteSpace(this string str) {
			return str.IsNullOrEmpty() || str.Trim().Length == 0;
		}

		public static bool IsNotNullOrWhiteSpace(this string str) {
			return !str.IsNullOrWhiteSpace();
		}

		public static double ToDouble(this string str) {
			double result;
			if (double.TryParse(str, out result)) {
				return result;
			}
			return double.NaN;
		}

		public static string Left(this string str, int length) {
			return str.Substring(0, length);
		}

		public static string Right(this string str, int length) {
			return str.Substring(str.Length - length);
		}

		public static bool EqualsIgnoreCase(this string str1, string str2) {
			return str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
		}

#if !SILVERLIGHT
		public static SecureString ToSecureString(this string str) {
			var result = new SecureString();
			var charArr = str.ToCharArray();
			foreach (var c in charArr) {
				result.AppendChar(c);
			}
			return result;
		}
#endif

	}
}