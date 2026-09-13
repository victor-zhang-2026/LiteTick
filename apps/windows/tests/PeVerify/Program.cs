using System.Diagnostics;
using System.Buffers.Binary;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;

if ((args.Length != 4 && args.Length != 5) || !File.Exists(args[0]))
{
    throw new ArgumentException("Usage: PeVerify <LiteTick.exe> <marketing-version> <build-number> <channel> [modern|legacy]");
}

var marketingVersion = args[1];
var buildNumber = args[2];
var releaseChannel = args[3];
var profile = args.Length == 5 ? args[4] : "modern";
var bytes = File.ReadAllBytes(args[0]);

using var stream = File.OpenRead(args[0]);
using var reader = new PEReader(stream);
var headers = reader.PEHeaders;
var peHeader = headers.PEHeader ?? throw new InvalidDataException("Missing PE header.");

Check(peHeader.Subsystem == Subsystem.WindowsGui, "Windows GUI subsystem");
Check(headers.SectionHeaders.Any(section => section.Name == ".rsrc" && section.SizeOfRawData > 0), "embedded Windows resources");
Check(HasResourceType(bytes, reader, 3), "embedded icon image resource");
Check(HasResourceType(bytes, reader, 14), "embedded icon group resource");
Check(HasResourceType(bytes, reader, 16), "embedded version resource");
Check(HasResourceType(bytes, reader, 24), "embedded application manifest");
Check(stream.Length > 0, "non-empty executable");

if (profile == "legacy")
{
    Check(headers.CoffHeader.Machine == Machine.I386, "legacy x86 machine type");
    Check(reader.HasMetadata, "legacy executable contains managed metadata");
    Check(peHeader.MajorSubsystemVersion < 6
        || (peHeader.MajorSubsystemVersion == 6 && peHeader.MinorSubsystemVersion <= 1),
        "legacy executable subsystem can load on Windows 7");
    Check(Contains(bytes, Encoding.UTF8.GetBytes("35138b9a-5d96-4fbd-8e2d-a2440225f93a")), "legacy manifest declares Windows 7 compatibility");
    var metadata = reader.GetMetadataReader();
    var references = metadata.AssemblyReferences
        .Select(handle => metadata.GetAssemblyReference(handle))
        .Select(reference => new
        {
            Name = metadata.GetString(reference.Name),
            reference.Version,
        })
        .ToList();
    Check(references.Any(reference =>
            reference.Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase)
            && reference.Version.Major == 2),
        "legacy executable targets the .NET Framework 2.0/3.5 CLR");
    Check(references.Any(reference =>
            reference.Name.Equals("System.Core", StringComparison.OrdinalIgnoreCase)
            && reference.Version == new Version(3, 5, 0, 0)),
        "legacy executable references the .NET Framework 3.5 System.Core assembly");
    Check(!references.Any(reference => reference.Name.Equals("System.Private.CoreLib", StringComparison.OrdinalIgnoreCase)),
        "legacy executable does not target modern .NET CLR");
}
else
{
    Check(profile == "modern", "known verification profile");
    Check(headers.CoffHeader.Machine == Machine.Amd64, "modern x64 machine type");
    Check(Contains(bytes, Encoding.UTF8.GetBytes("8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a")), "modern manifest declares current Windows compatibility");
}

var version = FileVersionInfo.GetVersionInfo(args[0]);
Check(Contains(bytes, Encoding.Unicode.GetBytes($"{marketingVersion}.{buildNumber}")), "embedded file version");
Check(Contains(bytes, Encoding.Unicode.GetBytes($"{marketingVersion}+build.{buildNumber}.{releaseChannel}")), "embedded product version");
if (OperatingSystem.IsWindows())
{
    Check(version.FileVersion == $"{marketingVersion}.{buildNumber}", "Windows file version API");
    Check(version.ProductVersion?.StartsWith(marketingVersion, StringComparison.Ordinal) == true, "Windows product version API");
}

Console.WriteLine($"PE verification passed ({profile}): {headers.CoffHeader.Machine} Windows GUI, {stream.Length} bytes, embedded version {marketingVersion} build {buildNumber} {releaseChannel}.");

static bool Contains(byte[] source, byte[] value)
{
    return source.AsSpan().IndexOf(value) >= 0;
}

static bool HasResourceType(byte[] bytes, PEReader reader, uint expectedType)
{
    var resourceRva = reader.PEHeaders.PEHeader?.ResourceTableDirectory.RelativeVirtualAddress ?? 0;
    var section = reader.PEHeaders.SectionHeaders.FirstOrDefault(candidate =>
        resourceRva >= candidate.VirtualAddress
        && resourceRva < candidate.VirtualAddress + Math.Max(candidate.VirtualSize, candidate.SizeOfRawData));
    if (resourceRva == 0 || section.Name is null)
    {
        return false;
    }

    var rootOffset = section.PointerToRawData + resourceRva - section.VirtualAddress;
    if (rootOffset < 0 || rootOffset + 16 > bytes.Length)
    {
        return false;
    }
    var namedCount = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(rootOffset + 12, 2));
    var idCount = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(rootOffset + 14, 2));
    var entryCount = namedCount + idCount;
    for (var index = 0; index < entryCount; index++)
    {
        var entryOffset = rootOffset + 16 + index * 8;
        if (entryOffset + 8 > bytes.Length)
        {
            return false;
        }
        var nameOrId = BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(entryOffset, 4));
        var isNamed = (nameOrId & 0x80000000) != 0;
        if (!isNamed && (nameOrId & 0xffff) == expectedType)
        {
            return true;
        }
    }
    return false;
}

static void Check(bool condition, string name)
{
    if (!condition)
    {
        throw new InvalidOperationException($"Failed: {name}");
    }
}
