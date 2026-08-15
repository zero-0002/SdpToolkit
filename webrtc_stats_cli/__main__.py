"""Command-line entry point: `webrtc-stats path/to/getstats.json`."""

from __future__ import annotations

import argparse
import json
import sys

from .report import build_report


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        prog="webrtc-stats",
        description="Summarize a WebRTC getStats() JSON dump.",
    )
    parser.add_argument(
        "path",
        nargs="?",
        default="-",
        help="JSON file produced by RTCPeerConnection.getStats(); '-' for stdin.",
    )
    args = parser.parse_args(argv)

    try:
        text = sys.stdin.read() if args.path == "-" else open(args.path, encoding="utf-8-sig").read()
    except OSError as exc:
        print(f"error: cannot read {args.path}: {exc}", file=sys.stderr)
        return 2

    try:
        raw = json.loads(text)
    except json.JSONDecodeError as exc:
        print(f"error: invalid JSON: {exc}", file=sys.stderr)
        return 2

    print(build_report(raw).render())
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
