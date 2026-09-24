# Urban Freerunner — Guía de instalación

## Descarga

[Descargar la build para Windows desde Google Drive](https://drive.google.com/file/d/1HQryq3KlAKxBWZM-q3UTxLFiFKtaSVCI/view?usp=sharing)

| Propiedad | Valor |
| --- | --- |
| Plataforma | Windows de 64 bits |
| Formato | Archivo RAR |
| Nombre | `Build.rar` |
| Tamaño | 130.274.825 bytes / 124,2 MiB |
| Ejecutable | `Build/TpProyectoUnity.exe` |
| Versión de Unity | `6000.3.22f1` |

## Verificación de la descarga

SHA-256 esperado:

```text
C3C90DB136661953963FED994B4DD0C1F4D01A890C27BD7C45093C9C7D094E00
```

Verificación con PowerShell:

```powershell
Get-FileHash .\Build.rar -Algorithm SHA256
```

El hash resultante debe coincidir exactamente con el valor esperado.

## Instalación

1. Descargar `Build.rar`.
2. Extraer el archivo completo con 7-Zip, WinRAR u otra herramienta compatible con RAR.
3. Mantener juntos `TpProyectoUnity.exe`, `TpProyectoUnity_Data`, `UnityPlayer.dll` y el resto de las carpetas extraídas.
4. Ejecutar `Build/TpProyectoUnity.exe`.

La aplicación es una build académica sin firma digital, por lo que Windows puede mostrar una advertencia de reputación. El ejecutable no debe separarse de su carpeta porque Unity necesita los archivos de datos adyacentes.

## Iniciar una partida multijugador

El flujo de Relay requiere conexión a Internet.

1. En la primera instancia, elegir la opción multijugador y crear una partida.
2. Copiar el código de acceso generado.
3. En una segunda instancia o computadora, elegir la opción multijugador e ingresar el código.
4. Cuando ambos jugadores estén conectados, el host inicia la partida.
5. Los dos clientes cargan la misma escena y reciben su propio jugador, cámara y controles.

El flujo validado para el proyecto utiliza exactamente dos jugadores.

## Controles

| Acción | Entrada |
| --- | --- |
| Movimiento | `W`, `A`, `S`, `D` |
| Correr | `Shift izquierdo` |
| Saltar | `Espacio` |
| Mirar | Mouse |
| Gancho | Botón derecho del mouse |

## Nota sobre el empaquetado

El archivo actual también contiene el directorio `BurstDebugInformation_DoNotShip` generado por Unity. No es necesario para jugar ni modifica el comportamiento del ejecutable; se conserva porque la build académica publicada se documenta tal como fue distribuida.

Durante el proyecto no se realizó una medición formal de requisitos mínimos o recomendados de hardware.
