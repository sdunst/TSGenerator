// add referenced assemblies here, like:
// #r "../bin/Debug/net461/win7-x64/TargetAssembly.dll"

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Common.Infrastructure.TSGenerate;

const string BasePath = @""; // specify a path here, if your generates files should be created somewhere else
IDictionary<Type, string> knownTypes = new Dictionary<Type, string>();
string currentFile = null;
StringBuilder fileBuilder;

StartFile("my-awesome-interfaces.d.ts");

// Add the classes, that should be exported here
// Interface<Namespace.ClassName>();
// Enums<Namespace.EnumName>();

FlushFile();

void Interface<T>()
{
  Type t = typeof(T);
  var filename = IsInterfaceNaming(t.Name) ? t.Name : ("I" + t.Name);

  knownTypes.Add(t, filename);
}

void WriteInterface(Type t, string filename)
{
  fileBuilder.AppendFormat("declare module {0} {{\r\n", t.Namespace);
  fileBuilder.AppendFormat("  interface {0} {{\r\n", filename);

  foreach (var mi in GetInterfaceMembers(t))
  {
    fileBuilder.AppendFormat("    {0}: {1};\r\n", ToCamelCase(mi.Name), GetTypeName(mi));
  }

  fileBuilder.AppendLine("  }");
  fileBuilder.AppendLine("}");
}

private bool IsInterfaceNaming(string name)
{
  return name.StartsWith("I") && name.Length > 1 && name[1] >= 'A' && name[1] <= 'Z';
}

IEnumerable<MemberInfo> GetInterfaceMembers(Type type)
{
  return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
      .Where(mi => mi.MemberType == MemberTypes.Field || mi.MemberType == MemberTypes.Property)
      .Where(mi => mi.GetCustomAttribute<IgnoreTSAttribute>(true) == null);
}

string ToCamelCase(string s)
{
  if (string.IsNullOrEmpty(s))
    return s;
  if (s.Length < 2)
    return s.ToLowerInvariant();
  return char.ToLowerInvariant(s[0]) + s.Substring(1);
}

Type GetType(MemberInfo mi)
{
  return (mi is PropertyInfo) ? ((PropertyInfo)mi).PropertyType : ((FieldInfo)mi).FieldType;
}

string GetTypeName(MemberInfo mi)
{
  return GetTypeName(GetType(mi));
}

string GetTypeName(Type t)
{
  if (t.IsPrimitive && !t.IsEnum)
  {
    if (t == typeof(bool))
      return "boolean";
    if (t == typeof(char))
      return "string";
    return "number";
  }
  if (t == typeof(System.DateTime))
    return "Date";
  if (t == typeof(System.Guid))
    return "string";
  if (t == typeof(System.TimeSpan))
    return "string";
  if (t == typeof(decimal))
    return "number";
  if (t == typeof(string))
    return "string";
  if (t.IsArray)
  {
    var at = t.GetElementType();
    return GetTypeName(at) + "[]";
  }
  if (typeof(System.Collections.IEnumerable).IsAssignableFrom(t))
  {
    var collectionType = t.GetGenericArguments()[0]; // all my enumerables are typed, so there is a generic argument
    return GetTypeName(collectionType) + "[]";
  }
  if (Nullable.GetUnderlyingType(t) != null)
  {
    return GetTypeName(Nullable.GetUnderlyingType(t));
  }

  if (knownTypes.ContainsKey(t))
    return $"{t.Namespace}.{knownTypes[t]}";
  if (t.IsEnum)
    return "number";

  return "any";
}

void Enums<T>() // Enums<>, since Enum<> is not allowed.
{
  Type t = typeof(T);
  knownTypes.Add(t, t.Name);
}

void WriteEnums(Type t)
{
  int[] values = (int[])Enum.GetValues(t);
  fileBuilder.AppendFormat("declare module {0} {{\r\n", t.Namespace);
  fileBuilder.AppendFormat("  export const enum {0} {{\r\n", t.Name);
  foreach (var val in values)
  {
    var name = Enum.GetName(t, val);
    fileBuilder.AppendFormat("    {0} = {1},\r\n", name, val);
  }

  fileBuilder.AppendLine("  }");
  fileBuilder.AppendLine("}");
}

void StartFile(string filename)
{
  currentFile = filename;
  fileBuilder = new StringBuilder();
}

void FlushFile()
{
  foreach (var entry in knownTypes)
  {
    if (entry.Key.IsEnum)
    {
      WriteEnums(entry.Key);
    }
    else
    {
      WriteInterface(entry.Key, entry.Value);
    }
  }

  var fileFolderPath = BasePath;
  var filePath = System.IO.Path.Combine(fileFolderPath, currentFile);

  System.IO.File.WriteAllText(filePath, fileBuilder.ToString());
}
