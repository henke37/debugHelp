using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {
	public class ATIP {
		byte WritePower;
		byte ReferenceSpeed;
		public bool DDCD;
		public bool UnrestrictedUse;
		public bool Rewriteable;

		public TrackTime LeadIn;
		public TrackTime LeadOut;

		byte[]? A1;
		byte[]? A2;
		byte[]? A3;

		[StructLayout(LayoutKind.Sequential)]
		internal struct DataHeader {
			byte LengthHi;
			byte LengthLo;
			byte Reserved1;
			byte Reserved2;

			internal UInt16 Length {
				get {
					return (UInt16)(LengthHi << 8 | LengthLo);
				}
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		internal unsafe struct DataBlock {

			byte RefSpeedWritePowerAndDDCDField;
			byte UnrestrictedUseField;
			byte TypesAndValidsField;
			byte Reserved1;
			byte LeadInMin;
			byte LeadInSec;
			byte LeadInFrame;
			byte Reserved2;
			byte LeadOutMin;
			byte LeadOutSec;
			byte LeadOutFrame;
			byte Reserved3;
			fixed byte A1[3];
			byte Reserved4;
			fixed byte A2[3];
			byte Reserved5;
			fixed byte A3[3];
			byte Reserved6;

			internal ATIP AsManaged() {
				return new ATIP() {
					DDCD = (RefSpeedWritePowerAndDDCDField & 0x08) != 0,
					ReferenceSpeed = (byte)(RefSpeedWritePowerAndDDCDField & 0x07),
					WritePower = (byte)(RefSpeedWritePowerAndDDCDField >> 4),

					UnrestrictedUse = (UnrestrictedUseField & 0x0040) != 0,
					Rewriteable = (TypesAndValidsField & 0x0040) !=0,

					LeadIn = new TrackTime(DecodeBCD(LeadInMin), DecodeBCD(LeadInSec),DecodeBCD(LeadInFrame)),
					LeadOut = new TrackTime(DecodeBCD(LeadOutMin), DecodeBCD(LeadOutSec),DecodeBCD(LeadOutFrame)),

					//A1 = (TypesAndValidsField & 0x04)!=0 ? A1 : null,
					//A2 = (TypesAndValidsField & 0x02)!=0 ? A2 : null,
					//A3 = (TypesAndValidsField & 0x01)!=0 ? A3 : null,
				};
			}

			private static byte DecodeBCD(byte b) { return (byte)((b >> 4) * 10 + (b & 0x0F)); }
		}

		enum CDRDiscType {
			CDR = 0,
			ANeg = 0b010,
			APos = 0b011,
			BNeg = 0b100,
			BPos = 0b101,
			CNeg = 0b110,
			cPos = 0b111
		}
	}
}
