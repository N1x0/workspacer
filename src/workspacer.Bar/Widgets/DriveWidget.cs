using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;


namespace workspacer.Bar.Widgets
{
    [Flags]
    public enum DriveProperty
    {
        Label = 1,
        Letter = 2,
        Format = 4,
        TotalSize = 8,
        FreeSpace = 16,
        FreeSpacePercentage = 32
    }
   
    public class DriveWidget : BarWidgetBase
    {

        public int Interval { get; set; } = 3000;
        public bool IsClickable { get; set; } = true;
        public bool IconsEnabled { get; set; } = true;
        public bool IsAscending { get; set; } = true;
        public List<DriveType> Types { get; set; } = new List<DriveType> { DriveType.Fixed, DriveType.Removable };
        public DriveProperty DriveDetails { get; set; } = DriveProperty.Letter | DriveProperty.FreeSpace;
        public Dictionary<DriveType, string> Icons { get; set; } = new Dictionary<DriveType, string>()
        {
        {DriveType.Fixed, "\uf0a0"},
            {DriveType.Removable, "\uf7c2" },
            {DriveType.Network, "\uf233" },
            {DriveType.CDRom, "\uf51f" },
            {DriveType.Ram,"\uf538" },
            {DriveType.Unknown, "\uf128" },
            {DriveType.NoRootDirectory, "\uf00d" }
        };

        private System.Timers.Timer _timer;

        public override IBarWidgetPart[] GetParts()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            List<IBarWidgetPart> parts = new List<IBarWidgetPart>();

            foreach (var drive in allDrives)
            {

                if (Types.Contains(drive.DriveType))
                {
                    parts.Add(CreatePart(drive, DriveDetails));
                }
            }

            if (IsAscending)
                parts.Reverse();

                   
            return parts.ToArray();
        }


        private IBarWidgetPart CreatePart(DriveInfo drive, DriveProperty DriveDetails)
        {
            
                if (IsClickable)
                    return Part(getDriveInfo(drive, DriveDetails), partClicked: () => Process.Start("explorer.exe", $"{drive.RootDirectory}"));
                else
                    return Part(getDriveInfo(drive, DriveDetails));
        }

        private string getDriveInfo(DriveInfo drive, DriveProperty DriveDetails)
        {
            var properties = new List<String>();
           
            if (drive.IsReady)
            {
                if (IconsEnabled)
                    properties.Add(Icons[drive.DriveType]);

                if (DriveDetails.HasFlag(DriveProperty.Label))
                    properties.Add(drive.VolumeLabel);

                if (DriveDetails.HasFlag(DriveProperty.Letter))
                    properties.Add(drive.Name.Remove(drive.Name.Length - 1, 1));

                if (DriveDetails.HasFlag(DriveProperty.Format))
                    properties.Add(drive.DriveFormat);

                if (DriveDetails.HasFlag(DriveProperty.TotalSize))
                    properties.Add((drive.TotalSize / Math.Pow(1024, 3)).ToString("0.0 GB"));

                if (DriveDetails.HasFlag(DriveProperty.FreeSpace))
                    properties.Add((drive.AvailableFreeSpace / Math.Pow(1024, 3)).ToString("0.0 GB"));

                if (DriveDetails.HasFlag(DriveProperty.FreeSpacePercentage))
                    properties.Add(((drive.AvailableFreeSpace / Math.Pow(1024, 3)) / (drive.TotalSize / Math.Pow(1024, 3))).ToString("0%"));

            }
              
                return string.Join(" ", properties);
        }


        public override void Initialize()
        {
            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
