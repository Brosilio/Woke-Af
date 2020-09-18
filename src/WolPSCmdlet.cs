using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Woke_Af
{
	[Cmdlet(VerbsCommunications.Send, "Wol")]
	[OutputType(typeof(string))]
	public class WolPSCmdlet : PSCmdlet
	{
		[Parameter(ParameterSetName = "mac", Mandatory = true, Position = 0)]
		public string MacAddress { get; set; }

		[Parameter]
		public int Port { get; set; } = 0;

		protected override void ProcessRecord()
		{
			MacAddress = MacAddress.Replace(':', '-');
			PhysicalAddress mac = PhysicalAddress.Parse(MacAddress);

			UdpClient client = new UdpClient()
			{
				EnableBroadcast = true
			};

			List<byte> magic = new List<byte>(102) { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
			for (int i = 0; i < 16; i++)
				magic.AddRange(mac.GetAddressBytes());

			int sent = client.Send(magic.ToArray(), magic.Count, new IPEndPoint(IPAddress.Broadcast, Port));

			WriteVerbose($"Broadcasted {sent} bytes. Recipient: {MacAddress} on port {Port}");

			client.Dispose();
		}
	}
}
