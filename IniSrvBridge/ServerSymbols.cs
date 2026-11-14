using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IniSrvBridge;

/// <summary>
/// Mena pre dopĺňanie v editore kodu: cleny typu <c>VlakS</c> (vyrazy statedgm.xml a TabTab ich pisu za bodkou vnutri
/// <c>With VlakS</c>) a typy servera, ktore vyrazy pouzivaju (vycty, <c>TabSupport</c>, <c>TabString</c>).
/// </summary>
internal static class ServerSymbols
{
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;

    // pomocne triedy servera, ktorych staticke cleny vyrazy volaju menom triedy
    private static readonly string[] HelperTypes = ["TabSupport", "TabString"];

    private static readonly HashSet<string> ObjectMembers = new(StringComparer.Ordinal) { "ToString", "Equals", "GetHashCode", "GetType" };

    private static readonly Dictionary<Type, string> VbNames = new()
    {
        [typeof(bool)] = "Boolean", [typeof(byte)] = "Byte", [typeof(short)] = "Short", [typeof(int)] = "Integer", [typeof(long)] = "Long",
        [typeof(float)] = "Single", [typeof(double)] = "Double", [typeof(decimal)] = "Decimal", [typeof(string)] = "String",
        [typeof(char)] = "Char", [typeof(DateTime)] = "Date", [typeof(object)] = "Object", [typeof(void)] = ""
    };

    /// <summary>
    /// JSON s menami: <c>{"symbols":[{"name","kind","signature"}]}</c>, druh <c>member</c>, <c>method</c> alebo <c>global</c>.
    /// </summary>
    /// <param name="serverDir">priecinok servera s IniSrv.exe (len sa cita)</param>
    public static string Read(string serverDir)
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, args) =>
        {
            var name = new AssemblyName(args.Name).Name;
            foreach (var extension in new[] { ".dll", ".exe" })
            {
                var file = Path.Combine(serverDir, name + extension);
                if (File.Exists(file))
                    return Assembly.LoadFrom(file);
            }

            return null;
        };

        var assembly = Assembly.LoadFrom(Path.Combine(serverDir, ServerLoad.ExecutableName));
        var types = Types(assembly);
        var symbols = new List<(string Name, string Kind, string Signature)>();

        if (types.FirstOrDefault(type => type.FullName == "IniSrv.VlakS") is { } train)
        {
            foreach (var group in train.GetMembers(PublicInstance).Where(Visible).GroupBy(member => member.Name))
            {
                var first = group.First();
                var kind = first is MethodInfo ? "method" : "member";
                symbols.Add((first.Name, kind, string.Join("\n", group.Select(Signature).Distinct())));
            }
        }

        foreach (var type in types.Where(type => type.IsPublic || type.IsNestedPublic))
        {
            var name = TypeName(type);
            if (type.IsEnum)
            {
                symbols.Add((name, "global", "Enum " + name));
                symbols.AddRange(Enum.GetNames(type).Select(value => (name + "." + value, "global", name + "." + value)));
            }
            else if (!type.IsNested && HelperTypes.Contains(type.Name))
            {
                symbols.Add((name, "global", "Class " + name));
                foreach (var group in type.GetMembers(PublicStatic).Where(Visible).GroupBy(member => member.Name))
                    symbols.Add((name + "." + group.Key, "global", string.Join("\n", group.Select(member => name + "." + Signature(member)).Distinct())));
            }
        }

        var json = new StringBuilder("{\"symbols\":[");
        for (var i = 0; i < symbols.Count; i++)
        {
            if (i > 0)
                json.Append(',');
            json.Append("{\"name\":").Append(Json(symbols[i].Name)).Append(",\"kind\":").Append(Json(symbols[i].Kind))
                .Append(",\"signature\":").Append(Json(symbols[i].Signature)).Append('}');
        }

        return json.Append("]}").ToString();
    }

    private static Type[] Types(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(type => type is not null).ToArray()!;
        }
    }

    private static bool Visible(MemberInfo member) => member switch
    {
        MethodInfo method => !method.IsSpecialName && !ObjectMembers.Contains(method.Name) && !method.IsGenericMethodDefinition,
        PropertyInfo or FieldInfo => true,
        _ => false
    };

    private static string Signature(MemberInfo member) => member switch
    {
        MethodInfo method => method.Name + "(" + string.Join(", ", method.GetParameters().Select(Parameter)) + ")" + As(method.ReturnType),
        PropertyInfo property => property.Name + (property.GetIndexParameters() is { Length: > 0 } index
            ? "(" + string.Join(", ", index.Select(Parameter)) + ")"
            : "") + As(property.PropertyType),
        FieldInfo field => field.Name + As(field.FieldType),
        _ => member.Name
    };

    private static string Parameter(ParameterInfo parameter) =>
        (parameter.IsOptional ? "Optional " : "") + parameter.Name + As(parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType()! : parameter.ParameterType);

    private static string As(Type type)
    {
        var name = VbType(type);
        return name.Length == 0 ? "" : " As " + name;
    }

    private static string VbType(Type type)
    {
        if (VbNames.TryGetValue(type, out var name))
            return name;
        if (type.IsArray)
            return VbType(type.GetElementType()!) + "()";
        if (type.IsGenericType)
        {
            var baseName = type.Name.Substring(0, type.Name.IndexOf('`'));
            return baseName + "(Of " + string.Join(", ", type.GetGenericArguments().Select(VbType)) + ")";
        }

        return TypeName(type);
    }

    private static string TypeName(Type type) => type.IsNested && type.DeclaringType is { } outer ? TypeName(outer) + "." + type.Name : type.Name;

    private static string Json(string value)
    {
        var text = new StringBuilder(value.Length + 2).Append('"');
        foreach (var c in value)
        {
            switch (c)
            {
                case '"':
                    text.Append("\\\"");
                    break;
                case '\\':
                    text.Append("\\\\");
                    break;
                case '\n':
                    text.Append("\\n");
                    break;
                default:
                    if (c < ' ')
                        text.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else
                        text.Append(c);
                    break;
            }
        }

        return text.Append('"').ToString();
    }
}
