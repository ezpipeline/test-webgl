# Test Unity WebGL build locally

To install the server as dotnet tool run the following:
```shell
dotnet tool install TestWebGL --global
```

Then you should be able to run the tool. You can check it by running the tool with --help or --version option:
```shell
test-webgl --help
```

```shell
Usage: test-webgl [--root <String>] [--port <Int32>] [--http] [--help] [--version]

TestWebGL

Options:
  -r, --root <String>    Optional path to WebGL Build root folder. Should contain index.html.
  -p, --port <Int32>     Optional web server port
  --http                 Use HTTP instead of HTTPS
  -h, --help             Show help message
  --version              Show version
```

To run the server from build output folder execute command with no arguments:
```shell
test-webgl
```

Or you can pass path to the build output directory via --path option:
```shell
test-webgl --root <path-to-build>
```
