using Dia2Lib;
using System;
using System.Collections.Generic;

namespace Henke37.DebugHelp {
	public class SymbolResolver {
		private DiaSource source;
		private IDiaSession session;

		public SymbolResolver(string pdbPath) {
			source=new DiaSource();
			source.loadDataFromPdb(pdbPath);
			source.openSession(out session);
		}

		public IDiaSymbol FindGlobal(string symbolName) {
			session.findChildren(session.globalScope, SymTagEnum.SymTagData, symbolName, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public IDiaSymbol FindClass(string className) {
			session.findChildren(session.globalScope, SymTagEnum.SymTagUDT, className, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public IDiaSymbol FindNestedClass(IDiaSymbol outerClass,string className) {
			outerClass.findChildren(SymTagEnum.SymTagUDT, className, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public IDiaSymbol FindTypeDef(string typeName) {
			session.findChildren(session.globalScope, SymTagEnum.SymTagTypedef, typeName, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public IDiaSymbol FindField(IDiaSymbol classSymb, string fieldName) {
			session.findChildren(classSymb, SymTagEnum.SymTagData, fieldName, (uint)NameSearchOptions.CaseSensitive, out var result);
			return result.Item(0);
		}

		public uint FieldOffset(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return (uint)field.offset;
		}

		public uint FieldSize(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return (uint)field.type.length;
		}

		public IDiaSymbol GetBaseClass(IDiaSymbol thisClass) {
			thisClass.findChildren(SymTagEnum.SymTagBaseClass, null, (uint)NameSearchOptions.None, out var result);
			return result.Item(0);
		}
	}
}
