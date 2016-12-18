using System;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;

namespace WireToggleEvent
{
	[ApiVersion(2, 00)]
	public class WireToggleEvent : TerrariaPlugin
	{
		#region Meta

		public override string Name => "Wire Toggle Event";
		public override string Author => "Newy";
		public override string Description => "Emits wire toggling events for use with other plugins";
		public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

		public delegate void DWireToggle(short x, short y);

		public static event DWireToggle HitSwitch;

		public WireToggleEvent(Main game) : base(game)
		{
			Order = 5;
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
#if DEBUG
			HitSwitch += (x, y) => Console.WriteLine($"{x}, {y}");
#endif
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
			}
			base.Dispose(disposing);
		}

		#endregion

		internal static void OnGetData(GetDataEventArgs e)
		{
			using (BinaryReader reader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
			{
				if (e.MsgID != PacketTypes.HitSwitch)
					return;

				short X = reader.ReadInt16();
				short Y = reader.ReadInt16();

				HitSwitch?.Invoke(X, Y);
			}
		}
	}
}