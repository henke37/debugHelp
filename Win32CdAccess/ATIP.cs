using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Henke37.Win32.CdAccess {
	public abstract class ATIP {
		byte WritePower;
		byte ReferenceSpeed;

		public bool UnrestrictedUse;
		public abstract bool Rewriteable { get; }

		public TrackTime LeadIn;
		public TrackTime LeadOut;

		internal ATIP () { }

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
				bool rewriteable = (TypesAndValidsField & 0x0040) != 0;

				bool a1Valid = (TypesAndValidsField & 0x04) != 0;
				bool a2Valid = (TypesAndValidsField & 0x02) != 0;
				bool a3Valid = (TypesAndValidsField & 0x01) != 0;

				if(rewriteable) {
					var cdrw = new ATIP.CDRW();
					cdrw.ReferenceSpeed = (byte)(RefSpeedWritePowerAndDDCDField & 0x07);
					cdrw.WritePower = (byte)(RefSpeedWritePowerAndDDCDField >> 4);

					cdrw.UnrestrictedUse = (UnrestrictedUseField & 0x0040) != 0;

					cdrw.LeadIn = new TrackTime(DecodeBCD(LeadInMin), DecodeBCD(LeadInSec), DecodeBCD(LeadInFrame));
					cdrw.LeadOut = new TrackTime(DecodeBCD(LeadOutMin), DecodeBCD(LeadOutSec), DecodeBCD(LeadOutFrame));
					return cdrw;
				} else {
					var cdr = new ATIP.CDR();
					cdr.ReferenceSpeed = (byte)(RefSpeedWritePowerAndDDCDField & 0x07);
					cdr.WritePower = (byte)(RefSpeedWritePowerAndDDCDField >> 4);

					cdr.UnrestrictedUse = (UnrestrictedUseField & 0x0040) != 0;

					cdr.LeadIn = new TrackTime(DecodeBCD(LeadInMin), DecodeBCD(LeadInSec), DecodeBCD(LeadInFrame));
					cdr.LeadOut = new TrackTime(DecodeBCD(LeadOutMin), DecodeBCD(LeadOutSec), DecodeBCD(LeadOutFrame));
					return cdr;
				}
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


		public class CDR : ATIP {
			public override bool Rewriteable => false;
		}

		public class CDRW : ATIP {
			public override bool Rewriteable => true;
		}
	}
}
