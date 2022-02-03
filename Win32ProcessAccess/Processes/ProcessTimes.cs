using Henke37.Win32.Base;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace Henke37.Win32.Processes {
	public struct ProcessTimes {
		public DateTime CreationTime;
		public DateTime ExitTime;
		public TimeSpan KernelTime;
		public TimeSpan UserTime;

		internal ProcessTimes(FILETIME creationTime, FILETIME exitTime, FILETIME kernelTime, FILETIME userTime) {
			CreationTime = creationTime.ToDateTime();
			ExitTime = exitTime.ToDateTime();
			KernelTime = kernelTime.ToTimeSpan();
			UserTime = userTime.ToTimeSpan();
		}

		public void Deconstruct(out DateTime CreationTime, out DateTime ExitTime) {
			CreationTime = this.CreationTime;
			ExitTime = this.ExitTime;
		}
		public void Deconstruct(out TimeSpan KernelTime, out TimeSpan UserTime) {
			KernelTime = this.KernelTime;
			UserTime = this.UserTime;
		}
		public void Deconstruct(out DateTime CreationTime, out DateTime ExitTime, out TimeSpan KernelTime, out TimeSpan UserTime) {
			CreationTime = this.CreationTime;
			ExitTime = this.ExitTime;
			KernelTime = this.KernelTime;
			UserTime = this.UserTime;
		}
	}
}