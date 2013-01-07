using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Net.Http.Headers;

namespace System.Net.Http.Formatting {

	public class XmlMediaTypeFormatter : MediaTypeFormatter {

		private static readonly Dictionary<Type, XmlSerializer> Serializers = new Dictionary<Type, XmlSerializer>();

		public XmlWriterSettings WriterSettings {
			get;
			private set;
		}

		public bool Indent {
			get;
			set;
		}

		public XmlMediaTypeFormatter() {
			this.SupportedMediaTypes = new Collection<string> { HttpMediaTypeNames.TextXml, HttpMediaTypeNames.ApplicationXml };
			this.WriterSettings = CreateDefaultWriterSettings();
		}

		private static XmlWriterSettings CreateDefaultWriterSettings() {
			return new XmlWriterSettings {
				CloseOutput = false,
				Indent = false
			};
		}

		public override bool CanReadType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return HasSerializeForType(type);
		}

		private static bool HasSerializeForType(Type type) {
			if (Serializers.ContainsKey(type)) {
				return true;
			}
			try {
				var serializer = new XmlSerializer(type);
				Serializers.Add(type, serializer);
				return true;
			}
			catch (Exception ex) {
				Debug.WriteLine("Can not create serializer for {0}, error is {1}", type, ex);
				return false;
			}
		}

		private static XmlSerializer GetSerializerForType(Type type) {
			if (HasSerializeForType(type)) {
				return Serializers[type];
			}
			return null;
		}

		public override bool CanWriteType(Type type) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			return HasSerializeForType(type);
		}

		public override Task<object> ReadFromStreamAsync(Type type, HttpContentHeaders headers, Stream stream) {
			var tcs = new TaskCompletionSource<object>();
			try {
				var serializer = GetSerializerForType(type);
				if (serializer == null) {
					tcs.TrySetException(new InvalidOperationException(string.Format("Can not create serializer for {0}", type)));
				}
				else {
					Task<object>.Factory.StartNew(() => serializer.Deserialize(stream))
						.ContinueWith(t => {
							if (t.IsFaulted) {
								tcs.TrySetException(t.Exception.GetBaseException());
							}
							else if (t.IsCanceled) {
								tcs.TrySetCanceled();
							}
							else {
								tcs.TrySetResult(t.Result);
							}
						}, TaskContinuationOptions.ExecuteSynchronously);
				}
			}
			catch (Exception ex) {
				tcs.TrySetException(ex);
			}
			return tcs.Task;
		}

		public override Task WriteToStreamAsync(Type type, object obj, HttpContentHeaders headers, Stream stream) {
			var tcs = new TaskCompletionSource<object>();
			try {
				var serializer = GetSerializerForType(type);
				if (serializer == null) {
					tcs.TrySetException(new InvalidOperationException(string.Format("Can not create serializer for {0}", type)));
				}
				else {
					Task.Factory.StartNew(() => {
						var settings = this.WriterSettings.Clone();
						settings.Indent = this.Indent;
						using (var xmlWriter = XmlWriter.Create(stream, settings)) {
							serializer.Serialize(xmlWriter, obj);
							xmlWriter.Flush();
						}
					}).ContinueWith(t => {
							if (t.IsFaulted) {
								tcs.TrySetException(t.Exception.GetBaseException());
							}
							else if (t.IsCanceled) {
								tcs.TrySetCanceled();
							}
							else {
								tcs.TrySetResult(null);
							}
						}, TaskContinuationOptions.ExecuteSynchronously);
				}
			}
			catch (Exception ex) {
				tcs.TrySetException(ex);
			}
			return tcs.Task;
		}
	}
}