using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace XComponent.MSBuild.Tasks
{
    public class ReplaceInFiles : Task
    {
        [Required]
        public ITaskItem[] Files { get; set; }

        [Required]
        public string Regex { get; set; }

        [Required]
        public string ReplacementText { get; set; }

        public override bool Execute()
        {
            try
            {
                var regex = new Regex(Regex);

                foreach (var file in Files)
                {
                    var path = file.GetMetadata("FullPath");
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    var text = File.ReadAllText(path);
                    text = regex.Replace(text, ReplacementText);
                    File.WriteAllText(path, text);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
        }
    }
}
