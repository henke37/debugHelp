using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Henke37.DebugHelp.Gdb {
	public class GdbClient : ProcessMemoryAccessor {
		TcpClient socket;

		public string Host = "localhost";
		public int Port = 2159;
		private bool acknowledgement = true;
		private NetworkStream socketStream;
		private MemoryStream readBuff;

		public string[] SupportStatus;
		private bool extendedMode = false;
		private bool vSane;

		private string pendingPacket;
		private string pendingNotification;
		private Dictionary<string, Action<string>> notificationMap;

		public event Action<StopPacket> ThreadStopped;

		public GdbClient() {
			socket = new TcpClient(AddressFamily.InterNetwork);
			readBuff = new MemoryStream();
			notificationMap = new Dictionary<string, Action<string>>();
			AddNotificationHandler("Stop", OnStopNotification);
		}

		public void Connect() {
			socket.Connect(Host, Port);
			socket.NoDelay = true;
			socketStream = socket.GetStream();

			TurnOffAcknowledgement();
			CheckVSane();
			CheckSupportedCommands();
			EnterExtendedMode();
			SendAllowedCommands();
		}

		#region public commands
		
		public override void ReadBytes(uint addr, uint size, byte[] readBuff) {
			string reply = SendCommand($"m{addr:x},{size:x}");
			if(reply == "") throw new UnsupportedCommandException();
			if(reply.StartsWith("E")) {
				throw new Exception();
			}

			int readLen = reply.Length / 2;

			if(readLen<size) {
				throw new IncompleteReadException();
			}

			for(int i = 0; i < readLen; ++i) {
				readBuff[i] = Convert.ToByte(reply.Substring(i * 2, 2), 16);
			}
		}

		public override void WriteBytes(byte[] srcBuff, uint dstAddr, uint size) {
			var sb = new StringBuilder();
			sb.AppendFormat($"M {0:x},{1:x}:", dstAddr, size);

			for(int i = 0; i < size; ++i) {
				sb.AppendFormat("{0:x2}", srcBuff[i]);
			}
			string reply = SendCommand(sb.ToString());
			if(reply == "") throw new UnsupportedCommandException();
			if(reply != "OK") {
				throw new Exception("Write failed");
			}
		}

		public uint CRCMemory(uint addr, uint size) {
			string reply = SendCommand("qCRC:{addr:x},{size:x}");
			if(reply == "") throw new UnsupportedCommandException();
			if(reply.StartsWith("E")) {
				throw new Exception();
			}
			return Convert.ToUInt32(reply.Substring(2, 4));
		}

		public uint SearchMemory(uint startAddr, byte[] pattern) {
			var sb = new StringBuilder();
			sb.AppendFormat("qSearch:memory:{0:x};{1:x};", startAddr, pattern.Length);

			for(int i = 0; i < pattern.Length; ++i) {
				sb.AppendFormat("{0:x2}", pattern[i]);
			}
			string reply = SendCommand(sb.ToString());
			if(reply == "") throw new UnsupportedCommandException();
			if(reply.StartsWith("E ")) {
				throw new Exception("Search errored");
			}

			if(reply == "0") return 0;

			return Convert.ToUInt32(reply.Substring(2), 16);
		}

		public void SetNonStop(bool enable = true) {
			string reply = SendCommand(enable ? "QNonStop:1" : "QNonStop:0");
			if(reply == "") throw new UnsupportedCommandException();
			if(reply != "OK") {
				throw new Exception();
			}
		}

		public Dictionary<int, string> GetProcessList() {
			string data = DoXFerRead("osdata", "processes");

			//Some idiot programs write decimal 20 (DC4) instead of hexadecimal 20 (boring ordinary space)
			data = data.Replace((char)20, ' ');

			var doc = new XmlDocument();
			doc.LoadXml(data);

			var procList = new Dictionary<int, string>();

			foreach(XmlNode itemNode in doc.FirstChild.ChildNodes) {
				XmlNode pidNode = itemNode.SelectSingleNode("column[@name='pid']");
				XmlNode commandNode = itemNode.SelectSingleNode("column[@name='command']");

				procList.Add(int.Parse(pidNode.InnerText), commandNode.InnerText.Trim());
			}
			return procList;
		}

		public List<RelocationInfo> GetRelocations() {
			var l = new List<RelocationInfo>();

			string reply = SendCommand("qOffsets");

			if(reply == "") {
				throw new UnsupportedCommandException();
			}

			foreach(string entry in reply.Split(';')) {
				string[] parts = entry.Split('=');
				l.Add(new RelocationInfo(parts[0], Convert.ToInt32(parts[1], 16)));
			}

			return l;
		}

		public StopPacket AttachToProcess(int pid) {
			if(!vSane) throw new InvalidOperationException();
			string reply = SendCommand($"vAttach;{pid:x}");

			if(reply == "") throw new UnsupportedCommandException();
			if(reply.StartsWith("E")) {
				throw new Exception();
			}

			if(reply == "OK") {
				return null;
			}

			return ParseStopPacket(reply);
		}
		#endregion


		#region Public nested types
		public class RelocationInfo {
			public string SectionName;
			public int Offset;

			public RelocationInfo(string SectionName, int Offset) {
				this.SectionName = SectionName;
				this.Offset = Offset;
			}
		}
		#endregion

		private string DoXFerRead(string @object, string annex) {
			int offset = 0;
			const int chunkSize = 1024*1024;

			string res = "";

			for(; ; ) {
				string reply = SendCommand($"qXfer:{@object}:read:{annex}:{offset:x},{chunkSize:x}");

				if(reply.StartsWith("E")) {
					throw new Exception();
				}

				res += reply.Substring(1);
			
				offset += reply.Length - 1;
				if(reply.StartsWith("l")) break;
			}

			return res;
		}

		private void CheckVSane() {
			string reply = SendCommand("vMustReplyEmpty");
			vSane = reply == "";
		}

		private void SendAllowedCommands() {
			//Strictly advisoary from the client perspective
			//The reply doesn't matter
			SendCommand("QAllow:WriteReg:0:WriteMem:1:InsertBreak:0:InsertTrace:0:InsertFastTrace:0:Stop:0");
		}

		private void EnterExtendedMode() {
			string reply=SendCommand("!");
			extendedMode = reply == "OK";
		}

		private void CheckSupportedCommands() {
			string reply=SendCommand("qSupported:multiprocess:QAllow");
			SupportStatus=reply.Split(';');
		}

		private void TurnOffAcknowledgement() {
			string reply = SendCommand("QStartNoAckMode");
			acknowledgement = reply != "OK";
		}

		private StopPacket ParseStopPacket(string reply) {
			string[] parts = reply.Substring(1).Split(';');
			switch(reply[0]) {
				case 'S':
				case 'T': {
					var reason = new StopReason();
					reason.Registers = new Dictionary<int, byte[]>();
					reason.Signal = Convert.ToInt32(reply.Substring(1, 2), 16);
					parts = reply.Substring(3).Split(';');

					foreach(var part in parts) {
						var subparts = part.Split(':');
						switch(subparts[0]) {
							case "thread":
								reason.ThreadId = ParseThreadId(subparts[1], out reason.ProcessId);
								break;
							case "core":
								reason.Core = Convert.ToInt32(subparts[1]);
								break;

							case "watch"://int reason arg
							case "rwatch":
							case "awatch":
							case "syscall_entry":
							case "syscall_return":
								reason.ReasonArg = Convert.ToInt32(subparts[1], 16);
								reason.Reason = subparts[0];
								break;

							case "replaylog"://string reason arg
							case "exec":


							case "fork"://Pid reason arg
							case "vfork":
							case "library"://No reason arg
							case "swbreak":
							case "hwbreak":
							case "create":
							case "vforkdone":
								reason.Reason = subparts[0];
								break;

							case ""://Split(;) artifact, each part ends with ";", including the last one
								break;

							default:
								if(Regex.IsMatch(subparts[0],"^[0-9A-F]{2}$")) {
									//we've got a register
									int regId = Convert.ToInt32(subparts[0],16);
									byte[] regVal = DecodeHexString(subparts[1]);
									reason.Registers.Add(regId, regVal);
								}
								break;
						}
					}

					return reason;
				}
					throw new NotImplementedException();

				case 'X': {
					if(parts.Length==1) {
						return new ProcessTermination(Convert.ToInt32(parts[0], 16));
					} else {
						return new ProcessTermination(
							Convert.ToInt32(parts[0], 16),
							Convert.ToInt32(parts[1].Split(':')[1])
						);
					}
				}
					

				case 'W':
				case 'w':
				case 'O':
				default:
					throw new NotImplementedException();
				case 'N':
					return new NoResumedThreads();
			}
		}

		private byte[] DecodeHexString(string v) {
			int nBytes = v.Length / 2;
			var buff = new byte[nBytes];
			for(int i=0;i< nBytes; ++i) {
				buff[i] = Convert.ToByte(v.Substring(2 * i, 2),16);
			}
			return buff;
		}

		private int ParseThreadId(string v, out int process) {
			process = 0;

			if(v.Contains(".")) {
				string[] parts = v.Split('.');
				string procStr = parts[0].Substring(1);
				if(procStr == "-1") process = -1;
				else if(procStr == "0") process = 0;
				else process = Convert.ToInt32(procStr, 16);
				v = parts[1];
			}

			if(v == "0") return 0;
			if(v == "-1") return -1;
			return Convert.ToInt32(v, 16);
		}

		#region Notification management
		//Must be reentry safe
		public void CheckForNotifications() {
			if(pendingPacket != null) return;

			if(pendingNotification != null) {
				DispatchPendingNotification();
				return;
			}

			if(!socketStream.DataAvailable) return;

			pendingPacket = ReadPacketInternal(false);

			if(pendingNotification != null) {
				DispatchPendingNotification();
			}
		}

		//Must be reentry safe
		private void DispatchPendingNotification() {
			//clear the pending notification first
			//since this function is reentry safe
			var currentNotification = pendingNotification;
			pendingNotification = null;

			int colonPos = currentNotification.IndexOf(':');
			if(colonPos < 0) throw new Exception();
			string name = currentNotification.Substring(0, colonPos);
			string @event = currentNotification.Substring(colonPos + 1);

			if(!notificationMap.TryGetValue(name, out Action<string> notificationDelegate)) {
				return;
			}
			notificationDelegate?.Invoke(@event);
		}

		private void AddNotificationHandler(string name,Action<string> handler) {
			notificationMap.TryGetValue(name, out Action<string> notificationDelegate);
			notificationMap[name] = notificationDelegate + handler;
		}
		private void RemoveNotificationHandler(string name, Action<string> handler) {
			if(notificationMap.TryGetValue(name, out Action<string> notificationDelegate)) {
				notificationMap[name] = notificationDelegate - handler;
			}
		}

		private void OnStopNotification(string @event) {
			if(ThreadStopped == null) return;
			var packet = ParseStopPacket(@event);
			ThreadStopped.Invoke(packet);
			
			for(; ;) {
				string reply = SendCommand("vStopped");

				if(reply == "OK") break;

				packet = ParseStopPacket(reply);
				ThreadStopped.Invoke(packet);
			} 

			CheckForNotifications();
		}

		#endregion

		#region Packet management
		private string SendCommand(string cmd) {
			byte[] wbuf = MakeCommandBuffer(cmd);
			socketStream.Write(wbuf, 0, wbuf.Length);
			ReadAck();
			return ReadPacket();
		}

		private string ReadPacket() {
			if(pendingPacket!=null) {
				string t = pendingPacket;
				pendingPacket = null;
				return pendingPacket;
			}
			return ReadPacketInternal();
		}

		private string ReadPacketInternal(bool waitForPacket=true) {
			startRead:
			readBuff.SetLength(0);

			byte b;
			b = ReadSocketByte();
			if(b != '$' && b != '%') {
				throw new IOException("Packet failed to start with $");
			}
			bool notification = b == '%';
			byte checksumComputed = 0;
			for(; ; ) {
				b = ReadSocketByte();
				if(b == '#') break;
				readBuff.WriteByte(b);
				checksumComputed += b;
			}

			byte checksumRead = ReadChecksum();

			if(acknowledgement && !notification) {
				if(checksumRead == checksumComputed) {
					socketStream.WriteByte((byte)'+');
				} else {
					socketStream.WriteByte((byte)'-');
					goto startRead;
				}
			}

			string packet = UnescapePacket(readBuff.ToArray());

			if(notification) {
				pendingNotification=packet;
				if(!socketStream.DataAvailable && !waitForPacket) {
					return null;
				}
				goto startRead;
			}

			return packet;
		}

		private byte ReadChecksum() {
			var buf = new byte[2];
			int readC=socketStream.Read(buf, 0, 2);
			if(readC!=2) {
				throw new EndOfStreamException();
			}
			return Convert.ToByte(Encoding.UTF8.GetString(buf), 16);
		}

		private byte ReadSocketByte() {
			int b = socketStream.ReadByte();
			if(b == -1) {
				throw new EndOfStreamException();
			}
			return (byte)b;
		}

		private void ReadAck() {
			if(!acknowledgement) return;
			int b = socketStream.ReadByte();
			if(b != '+') {
				throw new IOException("Bad packet acknowledge");
			}
		}

		private byte[] MakeCommandBuffer(string cmd) {
			cmd = EscapePacket(cmd);

			using(var ms = new MemoryStream(cmd.Length + 4)) {
				ms.WriteByte((byte)'$');

				var cmdBuf = Encoding.UTF8.GetBytes(cmd);
				ms.Write(cmdBuf, 0, cmdBuf.Length);

				byte checkSum = 0;
				foreach(var b in cmdBuf) {
					checkSum += b;
				}

				ms.WriteByte((byte)'#');
				ms.Write(Encoding.UTF8.GetBytes($"{checkSum:x2}"), 0, 2);

				return ms.ToArray();
			}
		}

		//TODO: Escape and unescape packet data
		private string EscapePacket(string cmd) {
			return cmd;
		}

		private string UnescapePacket(byte[] buff) {
			var sb = new StringBuilder();

			byte prevB=0;
			for(int i=0;i<buff.Length;++i) {
				byte b = buff[i];
				switch(b) {
					case (byte)'*':
						byte r = buff[++i];
						r -= 29;
						for(int j=0;j<r;++j) {
							sb.Append((char)prevB);
						}
						break;
					case 0x7D:
						b = (byte)(buff[++i] ^ 0x20);
						goto default;
					default:
						sb.Append((char)b);
						break;
				}
				prevB = b;
			}

			return sb.ToString();
		}

		[Serializable]
		public class UnsupportedCommandException : Exception {
			public UnsupportedCommandException() {
			}

			protected UnsupportedCommandException(SerializationInfo info, StreamingContext context) : base(info, context) {
			}
		}

		#endregion
	}
}
