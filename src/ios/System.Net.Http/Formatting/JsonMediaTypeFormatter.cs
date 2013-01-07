using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace System.Net.Http.Formatting {

	public class JsonMediaTypeFormatter : MediaTypeFormatter {

		public JsonSerializerSettings SerializerSettings {
			get;
			private set;
		}

		public bool Indent {
			get;
			set;
		}

		public JsonMediaTypeFormatter() {
			this.SupportedMediaTypes = new Collection<string> { HttpMediaTypeNames.TextJson, HttpMediaTypeNames.ApplicationJson };
			this.SerializerSettings = CreateDefaultJsonSerializerSettings();
		}

		public static JsonSerializerSettings CreateDefaultJsonSerializerSettings() {
			return new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Ignore,
				ObjectCreationHandling = ObjectCreationHandling.Auto,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
		}

		public override bool CanReadType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return true;
		}

		public override bool CanWriteType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return true;
		}

		public override Task<object> ReadFromStreamAsync(Type type, HttpContentHeaders headers, Stream stream) {
			return Task<object>.Factory.StartNew(() => {
				object result = null;
				try {
					using (var jsonReader = new JsonTextReader(new StreamReader(stream))) {
						var serializer = JsonSerializer.Create(this.SerializerSettings);
						result = serializer.Deserialize(jsonReader, type);
					}
				}
				catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine(ex);
				}
				return result;
			});
		}

		public override Task WriteToStreamAsync(Type type, object obj, HttpContentHeaders headers, Stream stream) {
			return Task.Factory.StartNew(() => {
				using (var jsonWriter = new JsonTextWriter(new StreamWriter(stream)) { CloseOutput = false }) {
					if (this.Indent) {
						jsonWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
					}
					var serializer = JsonSerializer.Create(this.SerializerSettings);
					serializer.Serialize(jsonWriter, obj);
					jsonWriter.Flush();
				}
			});
		}
	}
}