namespace System {
	public static class UriHelper {
		public static char HexUnescape(string pattern, ref int index) {
			if (index < 0 || index >= pattern.Length) {
				throw new ArgumentOutOfRangeException("index");
			}
			if (pattern[index] == '%' && pattern.Length - index >= 3) {
				char c = EscapedAscii(pattern[index + 1], pattern[index + 2]);
				if (c != (char)65535) {
					index += 3;
					return c;
				}
			}
			return pattern[index++];
		}

		internal static char EscapedAscii(char digit, char next) {
			if ((digit < '0' || digit > '9') && (digit < 'A' || digit > 'F') && (digit < 'a' || digit > 'f')) {
				return (char)65535;
			}
			int num = ((digit <= '9') ? (digit - '0') : (((digit <= 'F') ? (digit - 'A') : (digit - 'a')) + '\n'));
			if ((next < '0' || next > '9') && (next < 'A' || next > 'F') && (next < 'a' || next > 'f')) {
				return (char)65535;
			}
			return (char)((num << 4) + ((next <= '9') ? (next - '0') : (((next <= 'F') ? (next - 'A') : (next - 'a')) + '\n')));
		}
	}
}
