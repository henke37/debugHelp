using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Henke37.DebugHelp {
	public class IncompleteWriteException : Exception {

		public const int ErrorNumber = 299;

		public IncompleteWriteException() : base(Resources.WroteTooLittle) {
		}

		public IncompleteWriteException(Exception innerException) : base(Resources.WroteTooLittle, innerException) {
		}

		protected IncompleteWriteException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}
