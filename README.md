# SdpToolkit

A small C# (.NET 8) console tool + library that parses an SDP offer/answer
(RFC 4566) and prints its media sections, directions and codecs — useful when
debugging WebRTC negotiation.

## Build & run

```bash
dotnet run -- offer.sdp
# or pipe via stdin
cat answer.sdp | dotnet run
```

## Library

```csharp
using SdpToolkit;

var doc = SdpDocument.Parse(File.ReadAllText("offer.sdp"));
foreach (var m in doc.Media)
    Console.WriteLine($"{m.Kind}: {m.Codecs.Count} codecs");
```

MIT licensed.
