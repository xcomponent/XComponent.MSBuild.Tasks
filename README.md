# XComponent MSBuild Tasks

[![](http://slack.xcomponent.com/badge.svg)](http://slack.xcomponent.com/)

<img src="logo.png" width="64" height="64" />

This project contains custom MSBuild tasks developed by XComponent team.

## Invoking a task

The following example shows a project file invoking the ReplaceInFiles task:

```xml
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">  
    <UsingTask TaskName="XComponent.MSBuild.Tasks.ReplaceInFiles"   
        AssemblyFile="XComponent.MSBuild.Tasks.dll"/>  
  
    <Target Name="MyTarget">
        <ItemGroup>
            <FileToModify>
                <FullPath>.\TestA.txt</FullPath>
            </FileToModify>
            <FileToModify>
                <FullPath>.\TestB.txt</FullPath>
            </FileToModify>
        </ItemGroup>  
        <ReplaceInFiles 
                Files="@(FileToModify)"
                Regex="[a]"
                ReplacementText="b"/>  
    </Target>  
</Project>  
```

**ReplaceInFiles** task replaces a pattern described as a _Regex_ by the string provided in _ReplacementText_.  
So the example replaces 'a' caracters with 'b' caracters in _TestA.txt_ and _TestB.txt_ files.

## License
Apache License V2
