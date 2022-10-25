using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Henke37.Win32.CdAccess {

	public class CDText {

		public List<CdTextInfo> infos;

		public CDText() {
			infos = new List<CdTextInfo>();
		}

		internal static CDText FromBlocks(List<CdTextDataBlock> blocks) {
			var cdText = new CDText();

			var textBuff=new List<char>();

			textBuff.AddRange(blocks[0].Text);
			CdTextBlockType prevType = blocks[0].Type;
			int prevTrackNr = blocks[0].TrackNr;

			for(int blockIndex=1;blockIndex<blocks.Count;++blockIndex) {
				var block = blocks[blockIndex];

				var prevBuffSize = textBuff.Count;
				textBuff.AddRange(block.Text);

				if(block.Type != prevType || block.TrackNr != prevTrackNr) {

					int endPos = prevBuffSize - block.CharacterPosition;

					string txt=new string(textBuff.GetRange(0, endPos).ToArray());
					textBuff.RemoveRange(0, endPos);

					cdText.infos.Add(new CdTextInfo(prevType, prevTrackNr, txt));
				}

				prevType = block.Type;
				prevTrackNr = block.TrackNr;
			}

			if(textBuff.Count>0) {
				var txt= new string(textBuff.ToArray());
				cdText.infos.Add(new CdTextInfo(prevType, prevTrackNr, txt));
			}

			return cdText;
		}
	}

	public class CdTextInfo {
		public CdTextBlockType Type;
		public int TrackNr;
		public string Text;

		public CdTextInfo(CdTextBlockType type, int trackNr, string text) {
			this.Type = type;
			this.TrackNr = trackNr;
			this.Text = text;
		}

		public override string ToString() {
			switch(Type) {
				case CdTextBlockType.AlbumNameOrTrackTitle:
					if(TrackNr == 0) return $"Album: {Text}";
					return $"{TrackNr} {Text}";

				case CdTextBlockType.Performer:
					return $"{TrackNr} Performer: {Text}";
				case CdTextBlockType.Composer:
					return $"{TrackNr} Composer: {Text}";
				case CdTextBlockType.Arranger:
					return $"{TrackNr} Aranger: {Text}";
				case CdTextBlockType.Songwriter:
					return $"{TrackNr} Songwriter: {Text}";
				case CdTextBlockType.Message:
					return $"{TrackNr} MSG: {Text}";

				case CdTextBlockType.Genre:
					return $"{TrackNr} Genre";
				case CdTextBlockType.SizeInfo:
					return $"{TrackNr} Size";
				case CdTextBlockType.DiscID:
					return $"{TrackNr} DiscID";
				case CdTextBlockType.UPCEAN:
					return $"{TrackNr} UPC/EAN";

				default:
					return $"{TrackNr} {Type} {Text}";
			}
		}
	}

	internal class CdTextDataBlock {

		internal CdTextBlockType Type;
		internal int TrackNr;
		internal bool ExtensionFlag;
		internal int SequenceNumber;
		internal int CharacterPosition;
		internal int BlockNumber;

		internal string Text;

		[StructLayout(LayoutKind.Sequential)]
		internal unsafe struct Native {
			byte PackType;
			byte TrackAndExt;
			byte SequenceNumber;
			byte CharPosBlockAndUnicode;

			fixed byte Text[12];
			fixed byte CRC[2];

			public CdTextDataBlock AsManaged() {
				var block= new CdTextDataBlock();
				block.Type = (CdTextBlockType)PackType;
				block.TrackNr = TrackAndExt & 0x7F;
				block.ExtensionFlag = (TrackAndExt & 0x80) != 0;
				block.SequenceNumber = SequenceNumber;
				block.CharacterPosition = CharPosBlockAndUnicode & 0x0F; 
				block.BlockNumber = (CharPosBlockAndUnicode >> 4) & 0x07;

				bool isUnicode = (CharPosBlockAndUnicode & 0x80) != 0;

				fixed(byte* TextP = Text) {
					if(isUnicode) {
						block.Text = Encoding.Unicode.GetString(TextP, 12);
					} else {
						block.Text = Encoding.ASCII.GetString(TextP, 12);
					}
				}
				return block;
			}
		}

		public override string ToString() {
			return $"{TrackNr} {Type} -{CharacterPosition} {Text}";
		}
	}

	public enum CdTextBlockType : byte {
		AlbumNameOrTrackTitle = 0x80,
		Performer = 0x81,
		Songwriter = 0x82,
		Composer = 0x83,
		Arranger = 0x84,
		Message = 0x85,
		DiscID = 0x86,
		Genre = 0x87,
		TocInfo = 0x88,
		TocInfo2 = 0x89,
		UPCEAN = 0x8D,
		SizeInfo =0x8F
	}
}