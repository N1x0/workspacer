using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;


namespace workspacer.Bar.Widgets
{
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

        public int Interval { get; set; } = 1000;
        public bool HasAction { get; set; } = true;
        public bool HasIcons { get; set; } = false;        
        public List<DriveType> DriveTypes { get; set; } = new List<DriveType> { DriveType.Fixed, DriveType.Removable };
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
            var parts = new List<IBarWidgetPart>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (var drive in allDrives)
            {
                if (DriveTypes.Contains(drive.DriveType))
                {
                    parts.Add(CreatePart(drive, DriveDetails));
                }
            }
            
            return parts.ToArray();
        }


        private IBarWidgetPart CreatePart(DriveInfo drive, DriveProperty DriveDetails)
        {

            if (HasAction)
                return Part(getDriveInfo(drive, DriveDetails), partClicked: () => Process.Start("explorer.exe", $"{drive.RootDirectory}"));
            else
                return Part(getDriveInfo(drive, DriveDetails));
        }

        protected virtual string getDriveInfo(DriveInfo drive, DriveProperty DriveDetails)
        {
            var properties = new List<String>();
    
            if (drive.IsReady)
            {
                if (HasIcons)
                    properties.Add(Icons[drive.DriveType]);

                if ((DriveDetails & DriveProperty.Label) == DriveProperty.Label)
                    properties.Add(drive.VolumeLabel);

                if ((DriveDetails & DriveProperty.Letter) == DriveProperty.Letter)
                    properties.Add(drive.Name.Remove(drive.Name.Length - 1, 1));

                if ((DriveDetails & DriveProperty.Format) == DriveProperty.Format)
                    properties.Add(drive.DriveFormat);

                if ((DriveDetails & DriveProperty.TotalSize) == DriveProperty.TotalSize)
                    properties.Add((drive.TotalSize / Math.Pow(1024, 3)).ToString("0.0 GB"));

                if ((DriveDetails & DriveProperty.FreeSpace) == DriveProperty.FreeSpace)
                    properties.Add((drive.AvailableFreeSpace / Math.Pow(1024, 3)).ToString("0.0 GB"));

                if ((DriveDetails & DriveProperty.FreeSpacePercentage) == DriveProperty.FreeSpacePercentage)
                {
                    double freeSpace = (drive.AvailableFreeSpace / Math.Pow(1024, 3));
                    double totalSpace = (drive.TotalSize / Math.Pow(1024, 3));
                    properties.Add((freeSpace / totalSpace).ToString("0%"));
                }
                properties.Add(" ");
                return string.Join(" ", properties);
            }
            else
            {
                return string.Empty;
            }
        }


        public override void Initialize()
        {
            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;
        }
    }
}
