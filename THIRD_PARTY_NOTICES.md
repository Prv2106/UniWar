# Third-Party Notices

This project depends on third-party software. The table below summarizes the
licenses of the main components. Full license texts and copyright notices are
available in the respective upstream distributions.

## .NET Client

| Component | License | Notes |
| --- | --- | --- |
| .NET 8 runtime and base class library | MIT | Microsoft |
| .NET MAUI | MIT | Microsoft |
| Microsoft.Maui.Controls | MIT | Microsoft |
| Microsoft.Maui.Controls.Compatibility | MIT | Microsoft |
| Microsoft.Extensions.Logging.Debug | MIT | Microsoft |
| Google.Protobuf | BSD-3-Clause | Protocol Buffers runtime |
| Grpc.Net.Client | Apache-2.0 | gRPC client |
| Grpc.Tools | Apache-2.0 | Build-time code generation |
| Npgsql | PostgreSQL License | PostgreSQL driver |
| Python.Runtime | MIT | pythonnet runtime |
| MPowerKit.Lottie | MIT | Lottie animations |
| SkiaSharp | MIT | 2D graphics |
| SkiaSharp.Extended.UI.Maui | MIT | MAUI integration |
| Open Sans | Apache-2.0 | Bundled fonts under `Resources/Fonts` |

## Native Library

| Component | License | Notes |
| --- | --- | --- |
| nlohmann/json | MIT | Vendored as `UniWar/cppLibrary/json.hpp` |

## Python Server

| Component | License | Notes |
| --- | --- | --- |
| Python 3.9 | PSF License | Base Docker image |
| grpcio | Apache-2.0 | gRPC runtime |
| grpcio-tools | Apache-2.0 | Protocol Buffers code generation |
| PyMySQL | MIT | MySQL driver |
| bcrypt | Apache-2.0 | Password hashing |
| cryptography | Apache-2.0 OR BSD-3-Clause | Cryptographic backend |
| matplotlib | Matplotlib License | Charts; BSD-compatible |
| MySQL Community Server | GPLv2 | Database container; see upstream terms |

## Assets

The files under `UniWar/Resources/Images`, `UniWar/Resources/AppIcon`,
`UniWar/Resources/Splash`, and `UniWar/Resources/Raw` include logos, icons,
map artwork, and third-party illustrations. Their provenance is not documented
in the repository. The MIT License in `LICENSE` covers the original source code;
it does not automatically grant rights to third-party artwork. Verify the
license of each asset or replace it with an asset that may be redistributed
before publishing or distributing the application.

## Notes

- `functions_lib.dll` is built from this repository's C++ sources and the
  header-only nlohmann/json library.
- Dependency licenses can change between versions. Re-check them when upgrading
  packages or base images.
- This notice is informational and does not replace the full license texts
  distributed by the upstream projects.
