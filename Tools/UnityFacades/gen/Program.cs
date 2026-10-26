using System.Reflection.Metadata; using System.Reflection.PortableExecutable; using System.Text;
// usage: gen <UnityStubs.dll> <out.cs>  — emits TypeForwardedTo for every public top-level UnityEngine.* type
using var fs = File.OpenRead(args[0]); using var pe = new PEReader(fs); var md = pe.GetMetadataReader();
var sb = new StringBuilder("using System.Runtime.CompilerServices;\n");
foreach (var h in md.TypeDefinitions) { var t = md.GetTypeDefinition(h);
  if (!t.GetDeclaringType().IsNil) continue; // nested
  var ns = md.GetString(t.Namespace); var name = md.GetString(t.Name);
  if (!ns.StartsWith("UnityEngine")) continue;
  if ((t.Attributes & System.Reflection.TypeAttributes.VisibilityMask) != System.Reflection.TypeAttributes.Public) continue;
  var arity = name.IndexOf('`'); string cs = arity < 0 ? name : name[..arity] + "<" + new string(',', int.Parse(name[(arity+1)..]) - 1) + ">";
  sb.Append($"[assembly: TypeForwardedTo(typeof(global::{ns}.{cs}))]\n"); }
File.WriteAllText(args[1], sb.ToString()); Console.WriteLine($"forwarders: {sb.ToString().Count(c => c=='\n')-1}");
