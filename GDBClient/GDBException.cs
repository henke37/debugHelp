using System;
using System.Runtime.Serialization;

namespace Henke37.DebugHelp.Gdb {

	[Serializable]
	public class GDBException : Exception {
		public readonly int ErrCode;

		internal GDBException(int errCode) {
			this.ErrCode = errCode;
		}
		protected GDBException(SerializationInfo info, StreamingContext context) : base(info, context) {}

		internal static void ThrowFromReply(string reply) {
			throw FromReply(reply);
		}

		private static GDBException FromReply(string reply) {
			int errCode = Convert.ToInt32(reply.Substring(1), 16);
			return new GDBException(errCode);
		}
	}

	[Serializable]
	public class UnsupportedCommandException : Exception {
		public UnsupportedCommandException() {
		}

		protected UnsupportedCommandException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}
}