﻿using DIA;
using System;
using System.Collections.Generic;

namespace Henke37.DebugHelp.PdbAccess {
	public class SymbolResolver {

		private readonly List<PDBEntry> pdbEntries;

		private const int S_FALSE = 1;

		public SymbolResolver() {
			pdbEntries = new List<PDBEntry>();
		}

		public void AddPdb(string pdbPath) {
			var pdb = new PDBEntry(pdbPath);
			pdbEntries.Add(pdb);
		}

		public void AddPdb(string pdbPath, IntPtr loadAddr) {
			var pdb = new PDBEntry(pdbPath);
			pdb.LoadAddress=loadAddr;
			pdbEntries.Add(pdb);
		}

		public IDiaSymbol FindGlobal(string symbolName) {
			return FindSymbol(SymTagEnum.Data, symbolName);
		}

		public IntPtr GlobalAddress(string symbolName) {
			var symb = FindSymbol(SymTagEnum.Data, symbolName);
			return (IntPtr)symb.virtualAddress;
		}

		public IDiaSymbol FindClass(string className) {
			return FindSymbol(SymTagEnum.UDT, className);
		}

		public IDiaSymbol FindNestedClass(IDiaSymbol outerClass, string className) {
			var result = outerClass.findChildren(SymTagEnum.UDT, className, NameSearchOptions.CaseSensitive);
			return result.Item(0);
		}

		public IDiaSymbol FindTypeDef(string typeName) {
			return FindSymbol(SymTagEnum.Typedef, typeName);
		}

		public IDiaSymbol FindField(IDiaSymbol classSymb, string fieldName) {
			foreach(var pdb in pdbEntries) {
				var result = pdb.session.findChildren(classSymb, SymTagEnum.Data, fieldName, NameSearchOptions.CaseSensitive);
				if(result.count > 0) return result.Item(0);
			}
			throw new KeyNotFoundException();
		}

		public int FieldOffset(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return field.offset;
		}

		public IntPtr StaticFieldAddress(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return (IntPtr)field.virtualAddress;
		}

		public int FieldSize(IDiaSymbol classSymb, string fieldName) {
			IDiaSymbol field = FindField(classSymb, fieldName);
			return (int)field.type.length;
		}

		public IDiaSymbol GetBaseClass(IDiaSymbol thisClass) {
			var result = thisClass.findChildren(SymTagEnum.BaseClass, null, NameSearchOptions.None);
			return result.Item(0);
		}

		private IDiaSymbol FindSymbol(SymTagEnum symTag, string symbolName) {
			foreach(var pdb in pdbEntries) {
				var result = pdb.session.findChildren(pdb.session.globalScope, symTag, symbolName, NameSearchOptions.CaseSensitive);
				if(result.count > 0) return result.Item(0);
			}
			throw new KeyNotFoundException();
		}

		public IDiaSymbol FindFunctionAtAddr(IntPtr addr) {
			foreach(var pdb in pdbEntries) {
				var result = pdb.session.findSymbolByVA((ulong)addr, SymTagEnum.Function);
				return result;
			}
			throw new KeyNotFoundException();
		}

		public void AddressForVirtualAddress(IntPtr addr, out uint pISect, out uint pOffset) {
			foreach(var pdb in pdbEntries) {
				var success=pdb.session.addressForVA((ulong)addr, out pISect, out pOffset);
			}
			throw new KeyNotFoundException();
		}

		public int FrameDataForVirtualAddress(IntPtr addr, out IDiaFrameData? frame) {
			foreach(var pdb in pdbEntries) {
				var success = pdb.FrameDataForVirtualAddress(addr, out frame);
			}

			frame = null;
			return S_FALSE;
		}

		private class PDBEntry {

			public readonly IDiaDataSource source;
			public readonly IDiaSession session;
			private IDiaEnumFrameData? stackFrames;

			public PDBEntry(string pdbPath) {
				source = DiaLoader.CreateDiaSource();
				source.loadDataFromPdb(pdbPath);
				source.openSession(out session);
			}

			public IntPtr LoadAddress {
				set => session.loadAddress = (ulong)value;
				get => (IntPtr)session.loadAddress;
			}

			public int FrameDataForVirtualAddress(IntPtr addr, out IDiaFrameData frame) {
				InitFrameEnumerator();
				return stackFrames!.frameByVA((ulong)addr, out frame);
			}

			private void InitFrameEnumerator() {
				if(stackFrames != null) return;
				var tables = session.getEnumTables();
				foreach(IDiaTable table in tables) {
					stackFrames = table as IDiaEnumFrameData;
					if(stackFrames != null) return;
				}
				throw new Exception("Failed to locate the IDiaEnumFrameData interface!");
			}
		}
	}
}
