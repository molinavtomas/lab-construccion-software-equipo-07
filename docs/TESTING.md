# Urban Freerunner — Pruebas y CI

## Estrategia

El proyecto combina suites automatizadas de Unity Test Framework con pruebas manuales multijugador, trazabilidad de criterios de aceptación y evidencias reunidas durante cada sprint.

```text
Assets/Tests/
├── EditModeTests/
│   ├── RaceResultRulesTests.cs
│   ├── RaceStartRulesTests.cs
│   ├── Sprint3ConfigurationTests.cs
│   └── Sprint4RaceAutomatedTests.cs
└── PlayModeTests/
    ├── PlayerPlayModeTests.cs
    ├── SpeedBoostTests.cs
    ├── Sprint3GameplayTests.cs
    ├── Sprint4GameplayAutomatedTests.cs
    └── WallCollisionResponseTests.cs
```

El repositorio también conserva pruebas de ejemplo y de validación de infraestructura como parte de la evolución del proyecto. Por ese motivo, la presentación describe los comportamientos cubiertos en lugar de tratar cada método anotado como un requisito de producto independiente.

## Cobertura automatizada

| Área | Ejemplos |
| --- | --- |
| Preparación de la carrera | Cantidad requerida de jugadores, carga de escena, jugadores instanciados y rechazo por timeout |
| Reglas de resultado | Llegadas válidas, eventos duplicados o tardíos, resolución de ganador/perdedor y fin del tiempo |
| Ciclo del jugador | Configuración de propiedad, detección de caída, respawn en checkpoint y reinicio de velocidad |
| Movimiento | Carrera, salto, respuesta a colisiones y comportamiento en paredes |
| Mejoras de velocidad | Validación en servidor, tiempo compartido y recuperación de velocidad |
| Configuración | Escenas, prefabs, componentes y condiciones de aceptación requeridas |

## Suite consolidada

La [suite de QA y resultados](https://docs.google.com/spreadsheets/d/1HyE2hVOfY0tSBOdW02WvCWOdlR_yjt9laemHNITDczw/edit?usp=sharing) reúne en un único Google Sheet los libros de los Sprints 2, 3 y 4.

| Sprint | Casos | Registros | Aprobados | Fallidos | Bloqueados | Pendientes |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Sprint 2 | 18 | 18 | 14 | 3 | 0 | 1 |
| Sprint 3 | 30 | 30 | 27 | 1 | 2 | 0 |
| Sprint 4 | 27 | 27 | 27 | 0 | 0 | 0 |
| **Total** | **75** | **75** | **68** | **4** | **2** | **1** |

La clasificación de la fuente identifica 15 casos manuales, 13 parcialmente automatizables y 47 automatizables. El consolidado contiene además 38 criterios de aceptación y 44 enlaces directos a evidencias.

Los archivos originales presentan algunos cruces de IDs, saltos de numeración y una fecha que requiere confirmación. Esos puntos permanecen visibles en la pestaña `Hallazgos`; no se reasignaron ni completaron casos sin respaldo documental.

## Integración continua

El workflow [`.github/workflows/unity-tests.yml`](../.github/workflows/unity-tests.yml) utiliza Unity Test Runner de GameCI.

- Se ejecuta con cada push a `develop`.
- Se ejecuta en pull requests dirigidos a `develop`.
- Ejecuta EditMode y PlayMode como trabajos independientes de una matriz.
- Utiliza Git LFS durante el checkout.
- Conserva los resultados como artefactos de GitHub Actions incluso cuando un trabajo falla.

[Ver el workflow de pruebas de Unity](https://github.com/molinavtomas/lab-construccion-software-equipo-07/actions/workflows/unity-tests.yml)

## Ejecución local

1. Abrir el proyecto con Unity `6000.3.22f1`.
2. Abrir **Window → General → Test Runner**.
3. Ejecutar la suite de EditMode.
4. Ejecutar la suite de PlayMode.
5. Revisar los fallos junto con la consola de Unity y la configuración de la escena o prefab correspondiente.

## Evidencia multijugador

Las pruebas automatizadas cubren reglas deterministas y comportamiento de componentes. El flujo host/cliente completo también se ejecutó con dos instancias para validar la conexión mediante Relay, la propiedad de jugadores, la carga sincronizada, el cierre de la carrera y el resultado compartido.

![Dos instancias mostrando el resultado multijugador sincronizado](images/multiplayer-result-evidence.jpg)

La imagen corresponde a un cuadro de la evidencia de QA del Sprint 4 y muestra ambas instancias después de que el servidor resolvió el resultado de la carrera.
