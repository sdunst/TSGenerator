## TSGenerator

T4 Template, that creates TypeScript files from given .NET classes

## Usage

Include the assemblies, that contain the classes you want to generate:

```c#
<#@ template debug="true" hostSpecific="true" language="C#" #>
<#@ output extension=".ts" #>

<#@ Assembly Name="System.Core.dll" #>
<#@ assembly name="$(TargetDir)OtherProject.dll" #>
<#@ assembly name="$(TargetDir)OtherOtherProject.dll" #>
...
<#@ import namespace="System" #>
```

Specify, which classes should get exported:

```c#
<# StartFile("my-awesome-interfaces.d.ts"); #>
<# Interface<OtherProject.Model.SomeClass>(); #>
<# Interface<OtherProject.Model.SomeOtherClass>(); #>
<# Enums<OtherProject.Model.SomeEnum>(); #>
<# FlushFile(); #>
<# StartFile("other-awesome-interfaces.d.ts"); #>
<# Interface<OtherOtherProject.Model.SomeOtherOtherClass>(); #>
<# FlushFile(); #>
```

## Installation

Just download the GenerateTS.tt file and include it in your project.

## License

MIT
