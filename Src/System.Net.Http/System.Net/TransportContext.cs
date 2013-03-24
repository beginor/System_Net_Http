namespace System.Net {

	public class TransportContext {
		
		private TransportContext() {
		}

		public static TransportContext Default {
			get {
				return default(TransportContext);
			}
		}
	}
}