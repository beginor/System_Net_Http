using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http {

	public abstract class HttpMessageHandler : Disposable {
		
		protected internal abstract Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);

	}

}