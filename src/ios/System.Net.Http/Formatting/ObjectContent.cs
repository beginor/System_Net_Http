using System;
using System.IO;
using System.Threading.Tasks;

namespace System.Net.Http.Formatting {

	public class ObjectContent : HttpContent {

		private object _value;
		private readonly MediaTypeFormatter _formatter;

		public Type ObjectType {
			get;
			private set;
		}

		public MediaTypeFormatter Formatter {
			get {
				return this._formatter;
			}
		}

		public object Value {
			get {
				return this._value;
			}
			set {
				this.VerifyAndSetObject(value);
			}
		}

		public ObjectContent(Type type, object value, MediaTypeFormatter formatter)
			: this(type, value, formatter, null) {
		}

		public ObjectContent(Type type, object value, MediaTypeFormatter formatter, string mediaType) {
			if (type == null) {
				throw new ArgumentNullException("type");
			}
			if (formatter == null) {
				throw new ArgumentNullException("formatter");
			}
			this._formatter = formatter;
			this.ObjectType = type;
			this.VerifyAndSetObject(value);
			this._formatter.SetContentType(type, base.Headers, mediaType);
		}

		protected override Task SerializeToStreamAsync(Stream stream) {
			return this._formatter.WriteToStreamAsync(this.ObjectType, this.Value, base.Headers, stream);
		}

		private static bool IsTypeNullable(Type type) {
			return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		private void VerifyAndSetObject(object value) {
			if (value == null) {
				if (!IsTypeNullable(this.ObjectType)) {
					throw new InvalidOperationException(string.Format("{0} Cannot Use Null Value Type {1}", typeof(ObjectContent).Name, this.ObjectType.Name));
				}
			}
			else {
				var type = value.GetType();
				if (!this.ObjectType.IsAssignableFrom(type)) {
					throw new ArgumentException(string.Format("Object {0} And Type {1} Disagree", type.Name, this.ObjectType.Name), "value");
				}
				if (!this._formatter.CanWriteType(type)) {
					throw new InvalidOperationException(string.Format("ObjectContent Formatter {0} Cannot Write Type {1}", this._formatter.GetType().FullName, type.Name));
				}
			}
			this._value = value;
		}
	}

	public class ObjectContent<T> : ObjectContent {

		public ObjectContent(T value, MediaTypeFormatter formatter)
			: this(value, formatter, null) {
		}
	
		public ObjectContent(T value, MediaTypeFormatter formatter, string mediaType)
			: base(typeof(T), value, formatter, mediaType) {
		}
	}

}