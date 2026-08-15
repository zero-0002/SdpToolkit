using SdpToolkit;

// Entry point: `sdp-toolkit offer.sdp` (or pipe SDP via stdin).
var path = args.Length > 0 ? args[0] : null;
string input;
try
{
    input = path is null or "-" ? Console.In.ReadToEnd() : File.ReadAllText(path);
}
catch (IOException ex)
{
    Console.Error.WriteLine($"sdp-toolkit: {ex.Message}");
    return 1;
}

var doc = SdpDocument.Parse(input);
Console.WriteLine($"Session: \"{doc.SessionName}\" (v={doc.Version})");

if (doc.Media.Count == 0)
{
    Console.WriteLine("  (no media sections)");
    return 0;
}

for (var i = 0; i < doc.Media.Count; i++)
{
    var m = doc.Media[i];
    Console.WriteLine($"  m{i}: {m.Kind} {m.Direction} [{m.Protocol}] mid={m.Mid ?? "-"}");
    foreach (var c in m.Codecs)
    {
        var channels = c.Channels is null ? "" : "/" + c.Channels;
        Console.WriteLine($"      pt {c.PayloadType,-3} {c.Name}/{c.ClockRate}{channels}");
    }
}

return 0;
