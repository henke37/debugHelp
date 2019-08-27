using System;
using System.Security;
using System.Security.Permissions;

//https://docs.microsoft.com/en-us/dotnet/api/system.security.codeaccesspermission?view=netframework-4.8

namespace Henke37.DebugHelp.Win32.Permissions {
	[Serializable]
	sealed internal class ProcessPermission : CodeAccessPermission, IUnrestrictedPermission {

		private bool anyProcess = false;

		public ProcessPermission(PermissionState state) {
			switch(state) {
				case PermissionState.Unrestricted:
					anyProcess = true;
					break;
				case PermissionState.None:
					break;
				default:
					throw new ArgumentException("Invalid permission state.");
			}
		}

		public override IPermission Copy() {
			if(anyProcess) return new ProcessPermission(PermissionState.Unrestricted);

			throw new NotImplementedException();
		}

		public override void FromXml(SecurityElement elem) {
			throw new NotImplementedException();
		}

		public override IPermission Intersect(IPermission targetBase) {
			if(targetBase == null) return Copy();
			if(!(targetBase is ProcessPermission target)) throw new ArgumentException(String.Format("Argument_WrongType", this.GetType().FullName));
			if(target.IsSubsetOf(this)) return target.Copy();
			else if(this.IsSubsetOf(target)) return this.Copy();
			else return null;
		}

		public override IPermission Union(IPermission targetBase) {
			if(targetBase == null) return Copy();
			if(!(targetBase is ProcessPermission target)) throw new ArgumentException(String.Format("Argument_WrongType", this.GetType().FullName));
			if(target.IsSubsetOf(this)) return this.Copy();
			else if(this.IsSubsetOf(target)) return target.Copy();
			else return null;
		}

		public override bool IsSubsetOf(IPermission targetBase) {
			if(targetBase == null) return false;
			if(!(targetBase is ProcessPermission target)) throw new ArgumentException(String.Format("Argument_WrongType", this.GetType().FullName));
			if(target.anyProcess) return true;
			if(anyProcess) return false;
			throw new NotImplementedException();
		}

		public bool IsUnrestricted() {
			return anyProcess;
		}

		public override SecurityElement ToXml() {
			throw new NotImplementedException();
		}
	}
}
