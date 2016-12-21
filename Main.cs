using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

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

		public delegate void DWireToggle(WireToggleEventArgs e);

		public static event DWireToggle HitSwitch;

		public WireToggleEvent(Main game) : base(game)
		{
			Order = 5;
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);

#if DEBUG
			HitSwitch += e => Console.WriteLine($"{e.Position.X}, {e.Position.Y} from {e.Player.Name}");
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
			if (e.MsgID != PacketTypes.HitSwitch)
				return;

			using (var reader = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
			{
				short X = reader.ReadInt16();
				short Y = reader.ReadInt16();
				TSPlayer player = TShock.Players[e.Msg.whoAmI];

				HitSwitch?.Invoke(new WireToggleEventArgs(X, Y, player));
			}
		}
	}

	public class WireToggleEventArgs : EventArgs
	{
		public readonly Point Position;
		public readonly TSPlayer Player;

		public WireToggleEventArgs(short x, short y, TSPlayer plr)
		{
			Position = new Point(x, y);
			Player = plr;
		}
	}
}