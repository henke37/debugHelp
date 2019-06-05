using System;
using System.Runtime.Serialization;

namespace Henke37.DebugHelp {
	[Serializable]
	public class IncompleteReadException : Exception {

		public const int ErrorNumber = 299;

		public IncompleteReadException() : base(Resources.ReadTooLittle) {
		}
		public IncompleteReadException(Exception innerException) : base(Resources.ReadTooLittle, innerException) {
		}

		protected IncompleteReadException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}