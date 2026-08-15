# webrtc-stats-cli

A tiny, dependency-free CLI that turns a `RTCPeerConnection.getStats()` JSON dump
into a readable throughput / packet-loss / jitter report.

## Usage

```bash
python -m webrtc_stats_cli getstats.json
# or from stdin
cat getstats.json | python -m webrtc_stats_cli -
```

To capture a dump in the browser:

```js
const stats = await pc.getStats();
console.log(JSON.stringify([...stats.values()]));
```

## Install

```bash
pip install -e .
webrtc-stats getstats.json
```

## What it reports

- Inbound / outbound RTP grouped by media kind
- Codec (resolved via the `codec` records)
- Bytes transferred, packet-loss ratio, jitter (ms) and frame rate

MIT licensed.
