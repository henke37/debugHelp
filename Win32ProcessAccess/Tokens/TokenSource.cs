using System;

namespace Henke37.Win32.Tokens {
	public class TokenSource {

		public string SourceName;
		UInt64 SourceIdentifier;

		public TokenSource() { }
		internal TokenSource(Native native) {
			SourceIdentifier = native.SourceIdentifier;
			SourceName = native.GetSourceName();
		}

		internal unsafe struct Native {
			public fixed sbyte SourceNameBuff[8];
			public UInt64 SourceIdentifier;

			public string GetSourceName() {
				fixed(sbyte* buffP = SourceNameBuff) {
					return new string(buffP);
				}
			}

		}
	}
}
