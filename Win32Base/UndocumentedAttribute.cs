using System;

namespace Henke37.Win32.Base {
	/**
	 * A warning for stuff that's not officially documented and formally subject to arbitrary breakage.
	 * 
	 * Actualy policy may be everything between "public secret that will never change" to "has changed in every build".
	 * */
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	sealed public class UndocumentedAttribute : Attribute {
	}
}
