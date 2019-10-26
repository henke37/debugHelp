using DIA;
using System;
using System.Collections.Generic;

namespace Henke37.DebugHelp.PdbAccess {
	public class SymbolResolver {
		private readonly DiaSource source;
		private readonly IDiaSession session;
		private IDiaEnumFrameData? stackFrames;

		public SymbolResolver(string pdbPath) {
			source=new DiaSource();
			source.loadDataFromPdb(pdbPath);
			source.openSession(out session);
		}

		public void SetLoadAddress(IntPtr addr) {
			session.loadAddress = (ulong)addr;
		}

		public IDiaSymbol FindGlobal(string symbolName) {
			var result=session.findChildren(session.globalScope, SymTagEnum.Data, symbolName, NameSearchOptions.CaseSensitive);
			return result.Item(0);
		}

		public IDiaSymbol FindClass(string className) {
			var result = session.findChildren(session.globalScope, SymTagEnum.UDT, className, NameSearchOptions.CaseSensitive);
			return result.Item(0);
		}

		public IDiaSymbol FindNestedClass(IDiaSymbol outerClass,string className) {
			var result = outerClass.findChildren(SymTagEnum.UDT, className, NameSearchOptions.CaseSensitive);
			return result.Item(0);
		}

		public IDiaSymbol FindTypeDef(string typeName) {
			var result = session.findChildren(session.globalScope, SymTagEnum.Typedef, typeName, NameSearchOptions.CaseSensitive);
			return result.Item(0);
		}

		public IDiaSymbol FindField(IDiaSymbol classSymb, string fieldName) {
			var result = session.findChildren(classSymb, SymTagEnum.Data, fieldName, NameSearchOptions.CaseSensitive);
			return result.Item(0);
		}

		public int FieldOffset(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return field.offset;
		}

		public int FieldSize(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return (int)field.type.length;
		}

		public IDiaSymbol GetBaseClass(IDiaSymbol thisClass) {
			var result = thisClass.findChildren(SymTagEnum.BaseClass, null, NameSearchOptions.None);
			return result.Item(0);
		}

		public IDiaSymbol FindFunctionAtAddr(IntPtr addr) {
			var result = session.findSymbolByVA((ulong)addr, SymTagEnum.Function);
			return result;
		}

		public void AddressForVirtualAddress(IntPtr addr, out uint pISect, out uint pOffset) {
			session.addressForVA((ulong)addr, out pISect, out pOffset);
		}

		public IDiaFrameData FrameDataForVirtualAddress(IntPtr addr) {
			InitFrameEnumerator();
			return stackFrames!.frameByVA((ulong)addr);
		}

		private void InitFrameEnumerator() {
			if(stackFrames != null) return;
			var tables = session.getEnumTables();
			foreach(IDiaTable table in tables) {
				stackFrames = table as IDiaEnumFrameData;//BUG: Should be IDiaEnumFrameData
				if(stackFrames != null) return;
			}
			throw new Exception("Failed to locate the IDiaEnumFrameData interface!");
		}
	}
}
