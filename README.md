# ProcessMonitor

The utility checks the lifetime of the process and ends it if it lives for a long time.

### Requirements:
  - C# 8.0
  - .NET Framework 4.7.1
  - Visual Studio 2019

### Libraries used:
  - [CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils) - Command-line parsing API and utilities for console applications.
  - [xUnit](https://github.com/xunit/xunit) for testing
  - [Moq](https://github.com/moq/moq) for unit testing

### Usage
```
Usage: processmonitor [options] <ProcessName> <Lifetime> <Rate>

Arguments:
  ProcessName                             Required. Process name.
  Lifetime                                Required. Process lifetime (in minutes).
  Rate                                    Required. Process lifetime check frequency (in minutes).

Options:
  -?|-h|--help                            Show help information
  -w|--wait-closing                       Optional. Wait for normal closing process.
  -c|--closing-timeout <CLOSING_TIMEOUT>  Optional. Closing timeout (in seconds)
```