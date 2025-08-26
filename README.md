![AbyssMoth_CuteDI_Logo](https://github.com/user-attachments/assets/5052c6a8-681f-47e1-8a90-42830b2f1e8e)

# CuteDI

Лёгкий DI-фреймворк для Unity.
Основа — контейнер с поддержкой:

* биндингов по **типу**, **инстансу** и **фабрике**,
* **тегов** для параллельных реализаций,
* инъекций через **конструктор** (в том числе с атрибутами `[InjectionConstructor]` и `[Tag]`),
* контейнеров **проекта** и **сцены**,
* вспомогательных биндингов для **MonoBehaviour** (создание GO, префабы, захват существующих инстансов).

> Фреймворк сделан прежде всего для внутреннего использования в **AbyssMoth Studios**. Используйте на свой страх и риск; API может эволюционировать.

---

## Установка

1. Импортируйте пакет.
   Папка плагина окажется здесь:
   `Assets/Plugins/RimuruDev/CuteDI`

2. Откройте пример: `Assets/Plugins/RimuruDev/CuteDI/Example` — там готовые сцены **Boot**, **MainMenu**, **Gameplay**, **Other** и демонстрация всех видов биндинга.

---

## Быстрый старт

### 1) Проектный контейнер

Проектный контейнер вы создаёте **сами** в точке входа (как в примере):

```csharp
using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        private IProjectContext projectContext;
        private IGameNavigation navigation;

        private void Awake()
        {
            projectContext = new ProjectContext();
            projectContext.Register();
            projectContext.Resolve();

            navigation = projectContext.Container.Resolve<IGameNavigation>();

            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) navigation.GoToMainMenu();
            if (Input.GetKeyDown(KeyCode.Alpha2)) navigation.GoToGameplay();
            if (Input.GetKeyDown(KeyCode.Alpha3)) navigation.GoToOther();
        }

        private void OnDestroy() => projectContext?.Release();
    }
}
```

`ProjectContext` внутри создаёт `DIContainer`, делает базовые регистрации (например, `ICoroutineRunner`, `IGameNavigation`), и передаёт контейнер во фреймворк через `DiProvider.SetProject(...)`.
Такой подход даёт полный контроль над тем, **что** находится на уровне проекта.

---

### 2) Сценовой контейнер

На каждой сцене положите `SceneContext` (есть готовый `SceneContextBootstrap`, который гарантирует наличие контекста).
В `SceneContext` можно назначить **ScriptableObject-инсталлеры** — они вызываются в `Awake` и наполняют контейнер сцены.

Пример простого инсталлера меню:

```csharp
using UnityEngine;

namespace AbyssMoth.CuteDI.Example
{
    [CreateAssetMenu(fileName = "MainMenuInstaller", menuName = "DI/Scene Installers/MainMenu")]
    public sealed class MainMenuInstaller : SceneInstallerSO
    {
        [SerializeField] private GameObject menuRootPrefab;

        public override void Compose(in DIContainer scene, in DIContainer project)
        {
            scene.RegisterInstance(this).AsSingle().NonLazy();
            scene.RegisterType<IMenuService, MenuService>().AsSingle().NonLazy();
            scene.Register(c => new MenuViewModel(c.Resolve<IMenuService>())).AsSingle().NonLazy();

            if (menuRootPrefab)
                scene.BindPrefab<IMenuRoot, MenuRoot>(menuRootPrefab, isUI: true);
        }
    }
}
```

---

## Виды биндинга

### 1) По типу (рекомендуется)

Вдохновлено Reflex. Делает конструкторную инъекцию автоматически.

```csharp
scene.RegisterType<IEnemySpawner, EnemySpawner>().AsSingle().NonLazy();
scene.RegisterType(typeof(IAnalytics), typeof(DummyAnalytics)).AsSingle().NonLazy();
```

### 2) По инстансу

```csharp
scene.RegisterInstance(this).AsSingle().NonLazy();
scene.RegisterInstance(configSo).AsSingle().NonLazy();
```

### 3) По фабрике

```csharp
scene.Register(c => new GameplayController(
        c.Resolve<IEnemySpawner>(),
        project.Resolve<IGameNavigation>()))
     .AsSingle()
     .NonLazy();
```

### 4) Теги и несколько реализаций

Ключ в контейнере — `(tag, serviceType)`. Для параллельных реализаций используйте **теги**:

```csharp
scene.RegisterType<IClock, UtcClock>("utc").AsSingle().NonLazy();
scene.RegisterType<IClock, GameClock>("game").AsSingle().NonLazy();
```

Инъекция нужной реализации через `[Tag]`:

```csharp
public sealed class AttrConsumer
{
    public IStorage File { get; }
    public IClock Clock { get; }

    [InjectionConstructor]
    public AttrConsumer([Tag("file")] IStorage file, [Tag("utc")] IClock clock)
    {
        File = file;
        Clock = clock;
    }
}
```

Сбор всех реализаций интерфейса:

```csharp
scene.RegisterType<IProcessor, ProcA>("proc_a").AsSingle();
scene.RegisterType<IProcessor, ProcB>("proc_b").AsSingle();

scene.Register(c => new ProcessorAggregator(c.ResolveAll<IProcessor>().ToArray()))
     .AsSingle()
     .NonLazy();
```

### 5) Замена (replace)

Можно заменить привязку (удобно для моков в тестовой сцене):

```csharp
scene.RegisterType<IReplaceSample, ReplaceA>().AsSingle();
scene.Replace<IReplaceSample>(c => new ReplaceB()).AsSingle().NonLazy();
```

### 6) MonoBehaviour-биндинги

Утилиты `DiUnity` — для сцепления контейнера и компонентов:

```csharp
// Новый пустой GO с компонентом
scene.BindNewGo<IFoo, FooMono>("FooGo", tag: "foo_go");

// Инстанс из префаба (UI или обычный)
scene.BindPrefab<IFoo, FooMono>(widgetPrefab, parent, isUI: true, tag: "foo_widget");

// Только компонент (Self) без интерфейса:
scene.BindPrefabSelf<HUD>(hudPrefab, parent);
```

> Важно: если биндите **несколько** экземпляров одного `MonoBehaviour`, давайте **разные теги**, иначе будет коллизия ключей.

---

## Инъекции (конструкторы)

* Если у класса один конструктор — он используется.
* Если конструкторов несколько — можете пометить нужный `[InjectionConstructor]`.
* Для выбора по тегу используйте `[Tag("...")]` у параметра.

---

## Разрешение зависимостей

```csharp
var nav = project.Resolve<IGameNavigation>();
var ok  = scene.TryResolve<IFoo>(out var foo, tag: "foo_widget");
var has = scene.HasRegistration<IEnemySpawner>();
var all = scene.ResolveAll<IProcessor>(); // агрегирует через регистр/родителя
```

---

## `DiProvider`: жизнь контейнеров

* `DiProvider.Project` — хранит **проектный контейнер** (вы создаёте его сами в `Bootstrapper` / `ProjectContext`).
* При загрузке сцены `SceneContext` вызывает `DiProvider.SceneContextBuilder(parent, sceneName, containerName)` — создаётся **сценовой контейнер**, кэшируется по handle сцены и используется родителем **Project**.
* На `SceneManager.sceneUnloaded` контейнер сцены **Dispose()**-ится и снимается с кэша.
* Получение контейнеров:

  * активной сцены: `DiProvider.GetCurrentSceneContainer()`
  * по ссылке на сцену: `scene.GetSceneContainer()`
  * по имени: `DiProvider.GetSceneContainerBySceneName("Gameplay")`

Можно собрать сценовой контейнер **вручную** (если строите своё последовательное управление загрузкой): вызовите `DiProvider.SceneContextBuilder(projectContainer, sceneName)` в момент, когда сцена уже валидна (обычно после `LoadSceneAsync` и перед первым кадром).

---

## Отладка

* Инспектор `SceneContext` в Play Mode показывает реестр биндингов контейнера сцены.
* Инспектор инсталлеров (`SceneInstallerSO`) поддерживает предпросмотр через `PreviewHints()`.
* Окно **Tools → CuteDI → DI Debugger** — быстрый просмотр содержимого контейнеров (проект/сцена/Prefab Stage), фильтр по типу/тегу.

---

## Пример «витрины» всех биндингов

В сцене `Other` лежит `AllBindingsInstaller` — демонстрация:

* `RegisterType / RegisterInstance / Register(factory)`
* теги и `[Tag]`
* `ResolveAll<T>()`
* `Replace<T>()`
* `BindNewGo / BindPrefab / BindPrefabSelf`

Плюс в примере `SceneHintUI` (**большие подсказки на экране**) с клавишами:

* `[1]` → MainMenu
* `[2]` → Gameplay
* `[3]` → Other

---

## Ограничения и заметки

* Ключ регистрации — `(tag, serviceType)`. Для нескольких реализаций одного интерфейса используйте **теги**.
* `BindOnGo` должен получать **сценовый** экземпляр. Для ассетов и префабов используйте `BindPrefab*`.
* Контейнер сцены удаляется при выгрузке сцены (`Dispose()`); проектный контейнер живёт до конца приложения.
* Фреймворк **не потокобезопасен**; используйте в основном потоке Unity.

---

## Лицензия

MIT. Подробности — в `LICENSE`.
