using DIA;
using System;
using System.Collections.Generic;

namespace Henke37.DebugHelp.PdbAccess {
	public class SymbolResolver {
		private DiaSource source;
		private IDiaSession session;

		public SymbolResolver(string pdbPath) {
			source=new DiaSource();
			source.loadDataFromPdb(pdbPath);
			source.openSession(out session);
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
	}
}
