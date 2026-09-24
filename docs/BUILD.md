# Urban Freerunner — Build guide

## Download

[Download the Windows build from Google Drive](https://drive.google.com/file/d/1HQryq3KlAKxBWZM-q3UTxLFiFKtaSVCI/view?usp=sharing)

| Property | Value |
| --- | --- |
| Platform | Windows 64-bit |
| Format | RAR archive |
| File name | `Build.rar` |
| Size | 130,274,825 bytes / 124.2 MiB |
| Executable | `Build/TpProyectoUnity.exe` |
| Unity version | `6000.3.22f1` |

## Verify the download

Expected SHA-256:

```text
C3C90DB136661953963FED994B4DD0C1F4D01A890C27BD7C45093C9C7D094E00
```

PowerShell verification:

```powershell
Get-FileHash .\Build.rar -Algorithm SHA256
```

The resulting hash should match the expected value exactly.

## Installation

1. Download `Build.rar`.
2. Extract the entire archive with 7-Zip, WinRAR or another RAR-compatible utility.
3. Keep `TpProyectoUnity.exe`, `TpProyectoUnity_Data`, `UnityPlayer.dll` and the other extracted folders together.
4. Run `Build/TpProyectoUnity.exe`.

The executable is an unsigned academic build, so Windows may display a reputation warning. Do not move the executable out of its extracted folder because Unity needs the adjacent data files.

## Start a multiplayer match

An internet connection is required for the Relay flow.

1. On the first instance, choose the multiplayer option and create a game.
2. Copy the generated join code.
3. On a second instance or computer, choose the multiplayer option and enter the code.
4. When both players are connected, the host starts the game.
5. Both clients load the same race scene and receive their own player, camera and controls.

The validated portfolio flow uses exactly two players.

## Controls

| Action | Input |
| --- | --- |
| Move | `W`, `A`, `S`, `D` |
| Sprint | `Left Shift` |
| Jump | `Space` |
| Look | Mouse |
| Grappling hook | Right mouse button |

## Packaging note

The current archive also contains Unity's `BurstDebugInformation_DoNotShip` directory. It is not required to play and does not change the executable's behavior; it remains in this historical academic build so the published artifact is documented exactly as distributed.

Minimum and recommended hardware requirements were not formally profiled during the academic project.
