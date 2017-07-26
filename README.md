## TSGenerator

T4 Template or Roslyn-Script, that creates TypeScript files from given .NET classes

## Usage

### Roslyn Script

Include the assemblies, that contain the classes you want to generate:

```c#
#r "../bin/Debug/net461/win7-x64/OtherProject.dll"
#r "../bin/Debug/net461/win7-x64/OtherOtherProject.dll"
...
using System;
using System.Reflection;
```

Specify, which classes should get exported:

```c#
StartFile("my-awesome-interfaces.d.ts");
Interface<OtherProject.Model.SomeClass>();
Interface<OtherProject.Model.SomeOtherClass>();
Enums<OtherProject.Model.SomeEnum>();
FlushFile();
StartFile("other-awesome-interfaces.d.ts");
Interface<OtherOtherProject.Model.SomeOtherOtherClass>();
FlushFile();
```

You can mark members with the IgnoreTS-Attribute to not include them in the generated TypeScript File

```c#
[IgnoreTS]
public ISomeType PropertyToIgnore { get; set; }
```

### T4 Template

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
