using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace System.Net.Http.Formatting {

	public abstract class MediaTypeFormatter {

		public ICollection<string> SupportedMediaTypes {
			get;
			protected set;
		}

		public abstract bool CanReadType(Type type);

		public abstract bool CanWriteType(Type type);

		public virtual void SetContentType(Type type, HttpContentHeaders headers, string mediaType) {
			if (this.SupportedMediaTypes == null) {
				throw new InvalidOperationException(string.Format("{0} does not set support media types", base.GetType()));
			}
			headers.ContentType = this.SupportedMediaTypes.Contains(mediaType) ? mediaType : this.SupportedMediaTypes.First();
		}

		public abstract Task<object> ReadFromStreamAsync(Type type, HttpContentHeaders headers, Stream stream);

		public abstract Task WriteToStreamAsync(Type type, object obj, HttpContentHeaders headers, Stream stream);

	}

}