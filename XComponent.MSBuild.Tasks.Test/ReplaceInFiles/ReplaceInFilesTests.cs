using System;
using System.IO;
using Microsoft.Build.Framework;
using NFluent;
using NSubstitute;
using NUnit.Framework;

namespace XComponent.MSBuild.Tasks.Test.ReplaceInFiles
{
    [TestFixture]
    public class ReplaceInFilesTests
    {
        [Test]
        public void ReplaceInFilesTest()
        {
            var testFile = @"ReplaceInFiles\InputAssemblyInfo.txt";
            var testFileInfo = new FileInfo(testFile);
            var taskItem = Substitute.For<ITaskItem>();
            taskItem.GetMetadata("FullPath").Returns(testFileInfo.FullName);

            var replaceInFilesTask = new Tasks.ReplaceInFiles() {
                Files = new[] { taskItem },
                Regex = @"Version\(""(\d+)\.(\d+)(\.(\d+)\.(\d+)|\.*)""\)",
                ReplacementText = @"Version(""0.1.0.20041"")"
            };

            var taskResult = replaceInFilesTask.Execute();

            Check.That(taskResult).IsTrue();

            var expectedResultFile = @"ReplaceInFiles\ExpectedAssemblyInfo.txt";

            Check.That(FilesContentsMatch(testFile, expectedResultFile)).IsTrue();
        }

        private bool FilesContentsMatch(string firstFilePath, string secondFilePath)
        {
            var result = false;

            try
            {
                var firstFileContent = File.ReadAllText(firstFilePath);
                var secondFileContent = File.ReadAllText(secondFilePath);
                result = firstFileContent == secondFileContent;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }
    }
}
