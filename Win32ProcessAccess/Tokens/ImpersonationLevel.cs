using System;

namespace Henke37.Win32.Tokens {
	public enum ImpersonationLevel {
		Anonymous=0,
		Identification,
		Impersonation,
		Delegation
	}
}
