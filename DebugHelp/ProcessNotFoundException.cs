using System;
using System.Runtime.Serialization;

namespace Henke37.DebugHelp {
	[Serializable]
	public class ProcessNotFoundException : Exception {
		public ProcessNotFoundException() : base("Target process not found") {

		}
		protected ProcessNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}
