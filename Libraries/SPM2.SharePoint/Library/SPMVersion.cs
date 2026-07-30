using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SPM2.SharePoint
{
    public class SPMVersion
    {
        public const String SpfPath = @"SOFTWARE\Microsoft\Shared Tools\Web Server Extensions";

        /// <summary>
        /// Release year of the installed farm: 2013, 2016 or 2019. Zero for
        /// Subscription Edition, which is continuously updated and carries no year.
        /// Prefer <see cref="Edition"/> for anything user facing.
        /// </summary>
        public int Year { get; set; }

        /// <summary>Office major version of the installed farm: 15 or 16, or 0 if none found.</summary>
        public int Office { get; set; }

        /// <summary>
        /// Display label for the installed release: "2013", "2016", "2019" or
        /// "Subscription Edition". Empty when no farm was detected, or when the
        /// version is v16 but the build could not be read.
        /// </summary>
        public string Edition { get; set; }

        /// <summary>
        /// Full build of Microsoft.SharePoint.dll, for example 16.0.19725.20280.
        /// Null if it could not be determined.
        /// </summary>
        public Version BuildVersion { get; set; }

        public string Number { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }

        public SPMVersion()
        {
            Name = "SharePoint Manager";
            Number = "1.0.12.1106";
            Edition = String.Empty;

            using (var root = Registry.LocalMachine.OpenSubKey(SpfPath))
            {
                if (root != null)
                {
                    // Highest hive wins, and we stop at the first match. An upgraded
                    // server can retain a stale key from an earlier install - a 14.0
                    // key sitting next to 16.0 is common - so testing in ascending
                    // order and letting the last match win reports the wrong product.
                    if (!TryLoadHive(root, "16.0"))
                    {
                        TryLoadHive(root, "15.0");
                    }
                }
            }

            Title = (Edition.Length > 0) ? String.Format("{0} {1}", Name, Edition) : Name;
        }

        private bool TryLoadHive(RegistryKey root, string hive)
        {
            using (var key = root.OpenSubKey(hive))
            {
                if (key == null) return false;

                Office = int.Parse(hive.Split('.')[0], CultureInfo.InvariantCulture);
                BuildVersion = ReadBuildVersion(key);

                if (Office == 15)
                {
                    Year = 2013;
                    Edition = "2013";
                    return true;
                }

                // 2016, 2019 and Subscription Edition all report 16.0, so only the
                // build number separates them. Ranges taken from the SharePoint
                // update history: 2016 starts at 16.0.4327, 2019 at 16.0.10337 and
                // Subscription Edition at 16.0.14326.
                int build = (BuildVersion != null) ? BuildVersion.Build : 0;

                if (build >= 14000)
                {
                    Year = 0;
                    Edition = "Subscription Edition";
                }
                else if (build >= 10000)
                {
                    Year = 2019;
                    Edition = "2019";
                }
                else if (build >= 4000)
                {
                    Year = 2016;
                    Edition = "2016";
                }
                else
                {
                    // A v16 farm whose build we could not read or do not recognise.
                    // Leave the edition blank rather than guessing at it.
                    Year = 0;
                    Edition = String.Empty;
                }

                return true;
            }
        }

        private static Version ReadBuildVersion(RegistryKey key)
        {
            var location = key.GetValue("Location") as string;

            // Prefer the file version of Microsoft.SharePoint.dll. It matches the
            // build numbers Microsoft publishes in the update history
            // (16.0.19725.20280), which is what the ranges above are expressed in.
            if (!String.IsNullOrEmpty(location))
            {
                try
                {
                    var dll = Path.Combine(location, @"ISAPI\Microsoft.SharePoint.dll");
                    if (File.Exists(dll))
                    {
                        var info = FileVersionInfo.GetVersionInfo(dll);
                        return new Version(info.FileMajorPart, info.FileMinorPart,
                                           info.FileBuildPart, info.FilePrivatePart);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                }
            }

            // Fall back to the hive's own Version value. Note it is shaped
            // differently: SE reports 16.0.0.19725, putting the build in the
            // revision slot, so it cannot be compared against the published tables
            // without normalising.
            try
            {
                var raw = key.GetValue("Version") as string;
                Version parsed;
                if (!String.IsNullOrEmpty(raw) && Version.TryParse(raw, out parsed))
                {
                    return (parsed.Build == 0 && parsed.Revision > 0)
                        ? new Version(parsed.Major, parsed.Minor, parsed.Revision, 0)
                        : parsed;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return null;
        }
    }
}
