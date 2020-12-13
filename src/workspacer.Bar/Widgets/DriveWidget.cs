using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;


namespace workspacer.Bar.Widgets
{

    public class DriveWidget : BarWidgetBase
    {

        public int Interval { get; set; } = 100000;
        public bool HasAction { get; set; } = true;
        private System.Timers.Timer _timer;
        public DriveType VisibleDriveTypes { get; set; } = DriveType.Fixed | DriveType.Removable;
        public DriveProperty DriveInformation { get; set; } = DriveProperty.Letter | DriveProperty.FreeSpace;


        public enum DriveProperty
        {
            Label = 1,
            Letter = 2,
            Format = 4,
            TotalSize = 8,
            FreeSpace = 16,
            FreeSpacePercentage = 32
        }


        public override IBarWidgetPart[] GetParts()
        {
            var parts = new List<IBarWidgetPart>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (var drive in allDrives)
            {
                if (drive.DriveType.Equals(VisibleDriveTypes))
                {
                    parts.Add(CreatePart(drive, DriveInformation));
                }
            }

            return parts.ToArray();
        }


        private IBarWidgetPart CreatePart(DriveInfo drive, DriveProperty DriveInformation)
        {

            if (HasAction)
                return Part(getDriveInfo(drive, DriveInformation), null, null, () => Process.Start("explorer.exe", $"{drive.RootDirectory}"));
            else
                return Part(getDriveInfo(drive, DriveInformation));
        }


        protected virtual string getDriveInfo(DriveInfo drive, DriveProperty DriveInformation)
        {
            var properties = new List<String>();

            if (drive.IsReady)
            {

                if ((DriveInformation & DriveProperty.Label) == DriveProperty.Label)
                    properties.Add(drive.VolumeLabel);

                if ((DriveInformation & DriveProperty.Letter) == DriveProperty.Letter)
                    properties.Add(drive.Name.Remove(drive.Name.Length - 1, 1));

                if ((DriveInformation & DriveProperty.Format) == DriveProperty.Format)
                    properties.Add(drive.DriveFormat);

                if ((DriveInformation & DriveProperty.TotalSize) == DriveProperty.TotalSize)
                    properties.Add((drive.TotalSize / Math.Pow(1024, 3)).ToString("0.0 GB"));

                if ((DriveInformation & DriveProperty.FreeSpace) == DriveProperty.FreeSpace)
                    properties.Add((drive.AvailableFreeSpace / Math.Pow(1024, 3)).ToString("0.0 GB"));

                if ((DriveInformation & DriveProperty.FreeSpacePercentage) == DriveProperty.FreeSpacePercentage)
                {
                    double freeSpace = (drive.AvailableFreeSpace / Math.Pow(1024, 3));
                    double totalSpace = (drive.TotalSize / Math.Pow(1024, 3));
                    properties.Add((freeSpace / totalSpace).ToString("0%"));
                }

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
