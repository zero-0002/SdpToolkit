namespace SdpToolkit;

/// <summary>A resolved <c>a=rtpmap</c> codec entry.</summary>
public sealed record Codec(string PayloadType, string Name, string ClockRate, string? Channels);

/// <summary>A single <c>m=</c> media section of an SDP document.</summary>
public sealed class MediaSection
{
    public string Kind { get; init; } = "";
    public string Port { get; init; } = "";
    public string Protocol { get; init; } = "";
    public string Direction { get; set; } = "sendrecv";
    public string? Mid { get; set; }
    public List<Codec> Codecs { get; } = new();
}

/// <summary>A minimal SDP (RFC 4566) parser aimed at WebRTC negotiation triage.</summary>
public sealed class SdpDocument
{
    public string Version { get; private set; } = "";
    public string SessionName { get; private set; } = "";
    public List<MediaSection> Media { get; } = new();

    public static SdpDocument Parse(string raw)
    {
        var doc = new SdpDocument();
        MediaSection? current = null;

        foreach (var rawLine in raw.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            if (line.Length < 2 || line[1] != '=') continue;

            var key = line[0];
            var value = line[2..].Trim();

            switch (key)
            {
                case 'v':
                    doc.Version = value;
                    break;
                case 's':
                    doc.SessionName = value;
                    break;
                case 'm':
                    current = ParseMedia(value);
                    doc.Media.Add(current);
                    break;
                case 'a' when current is not null:
                    ApplyAttribute(current, value);
                    break;
            }
        }

        return doc;
    }

    private static MediaSection ParseMedia(string value)
    {
        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new MediaSection
        {
            Kind = parts.ElementAtOrDefault(0) ?? "",
            Port = parts.ElementAtOrDefault(1) ?? "",
            Protocol = parts.ElementAtOrDefault(2) ?? "",
        };
    }

    private static void ApplyAttribute(MediaSection media, string value)
    {
        if (value is "sendrecv" or "sendonly" or "recvonly" or "inactive")
        {
            media.Direction = value;
        }
        else if (value.StartsWith("mid:", StringComparison.Ordinal))
        {
            media.Mid = value["mid:".Length..];
        }
        else if (value.StartsWith("rtpmap:", StringComparison.Ordinal))
        {
            media.Codecs.Add(ParseRtpmap(value["rtpmap:".Length..]));
        }
    }

    private static Codec ParseRtpmap(string value)
    {
        // "96 VP8/90000" or "111 opus/48000/2"
        var space = value.Split(' ', 2);
        var pt = space[0];
        if (space.Length < 2) return new Codec(pt, "", "", null);

        var enc = space[1].Split('/');
        return new Codec(
            pt,
            enc.ElementAtOrDefault(0) ?? "",
            enc.ElementAtOrDefault(1) ?? "",
            enc.ElementAtOrDefault(2));
    }
}
