<p align="center">
<img src="Screenshots~/Preview.jpg">
</p>

# Parameter System

Простая и удобная в использовании система статов/параметров

## Зависимости

- [Scellecs.Collections](https://github.com/scellecs/collections)
- [Qw1nt.SelfId](https://github.com/Qw1nt/unity.self-id)

## Установка

Через <b>.unitypackage</b>

## Основное

1) Одно хранилище для всех параметров объекта - `Docker`,
2) Каждый параметр имеет набор значений, из которых происходит его расчёт,
3) Каждый `Docker` может иметь один родительский `Docker` и множество дочерних,
4) Данные родительского `Docker` влияют на расчёт дочерних,
5) Параметры поддерживают формулы расчёта, уникальные для каждого `Docker`

## Параметр

### Создание 

1) Добавление нового Id; см. [unity.self-id](https://github.com/Qw1nt/unity.self-id)
2) Объявление нового типа  `MovementSpeed`

```csharp
using Parameters.Runtime.Attributes;

namespace Runtime
{
    [Parameter(typeof(float))]
    public partial struct MovementSpeed
    {
        
    }
}
```

> [!IMPORTANT]
> Аргумент в `[Parameter(typeof(float))]` отвечает за то, к какому типу будет приводится параметр.

3) Создание ScriptableObject параметра: `Create -> Parameters -> Parameter Data`
4) Задать ранее созданный ID
5) Задать инициализатор

### Вид Scriptable Object'а 
<img src="./Screenshots~/ParameterData.jpg" alt="img"/>

> [!IMPORTANT]
> Инициализатор всегда имеет такое же название, что и параметр

### Инициализация
На любой объект сцены нужно добавить `ParameterInitializerMonoProvider`.

После запуска сцены все валидные параметры проинициализируются и смогут быть использованы.


## Defines

``PARAMETERS_TRI_INSPECTOR`` - Интеграция с [TriInspector](https://github.com/codewriter-packages/Tri-Inspector)

``PARAMETERS_UINITY_LOCALIZATION`` - Интеграция
с [Unity.Localization](https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/index.html)

```PARAMETERS_UNITASK``` - Интеграция с [UniTask](https://github.com/Cysharp/UniTask)