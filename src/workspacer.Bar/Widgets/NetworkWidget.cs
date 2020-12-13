using System.Net.NetworkInformation;
using System.Collections.Generic;
namespace workspacer.Bar.Widgets
{
    public class NetworkWidget : BarWidgetBase
    {
        public string WifiSymbol { get; set; } = "Wifi";
        public string EthernetSymbol { get; set; } = "Ethernet";
        public int Interval { get; set; } = 5000;

        private System.Timers.Timer _timer;

        public override IBarWidgetPart[] GetParts()
        {
            var parts = new List<IBarWidgetPart>();
            string symbol;
            foreach (NetworkInterface netInt in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (netInt.OperationalStatus == OperationalStatus.Up && netInt.NetworkInterfaceType != (NetworkInterfaceType.Tunnel | NetworkInterfaceType.Loopback))
                {

                    if (netInt.Name == "Wifi")
                        symbol = WifiSymbol;
                    else if (netInt.Name == "Ethernet")
                        symbol = EthernetSymbol;
                    else
                        symbol = netInt.Name;

                    parts.Add(Part(text: symbol));
                }
            }

            return parts.ToArray();
        }


        public override void Initialize()
        {
            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
