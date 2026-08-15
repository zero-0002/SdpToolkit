"""webrtc-stats-cli: turn a getStats() JSON dump into a readable report."""

from .report import Report, build_report

__all__ = ["Report", "build_report"]
__version__ = "0.1.0"
