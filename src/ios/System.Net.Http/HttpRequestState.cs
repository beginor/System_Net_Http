using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	internal class HttpRequestState {

		internal HttpWebRequest WebRequest;
		internal TaskCompletionSource<HttpResponseMessage> Tcs;
		internal CancellationToken CancellationToken;
		internal HttpRequestMessage RequestMessage;
		internal Stream RequestStream;

	}
}