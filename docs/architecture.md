# Architecture

## Overview

Internet Monitor is split into several applications:

- `InternetMonitor.CLI` - Command-line interface
- `InternetMonitor.Core` - Core application logic
- `InternetMonitor.Service` - Background monitoring service
- SQLite - Persistent data storage

## System Architecture

The following shows the overall system architecture.

[Open interactive architecture](../architecture/)

## Core Architecture

`InternetMonitor.Core` contains the main monitoring, data and export services.

```text
MonitorService
    │
    ├── ConnectivityService
    └── SpeedTestService
             │
             ▼
         DataService
             │
             ▼
           SQLite

ExportService
    │
    ├── JsonExporter
    ├── CsvExporter
    └── PdfReportExporter

```


    