# UML Diagrams

All diagrams below describe the current live architecture of the refactored solution. They intentionally exclude `OldProgram.cs` from the runtime model.

The PlantUML blocks are written to be source controlled and maintained alongside the code. Each diagram is based on direct code inspection rather than inferred architecture alone.

## 1. System Context And Main Components

```plantuml
@startuml
title BeautifulClient current architecture and external dependencies
left to right direction
skinparam componentStyle rectangle

actor User

rectangle "BeautifulClient console app" {
  component "Program\ncomposition root" as Program
  component "App" as App
  component "ConsoleHost" as ConsoleHost
  component "DashboardController" as DashboardController
  component "DashboardPage" as DashboardPage
  component "DashboardRenderer" as DashboardRenderer
  component "CommandParser" as CommandParser
  component "CommandExecutor" as CommandExecutor
  component "UniversalApiFacade" as UniversalApiFacade
  component "LocalAdapter" as LocalAdapter
  component "RemoteAdapter" as RemoteAdapter
  component "ApiResultPipeline" as ApiResultPipeline
  component "ObjectSetterPipeline" as ObjectSetterPipeline
  component "SerilogQueSink\nstatic in process queue" as SerilogQueSink
}

database "User Secrets\nApiSettings ApiKey" as UserSecrets
file "appsettings.json" as AppSettings
folder "logs/app-log-*.txt" as LogFiles
component "HardwarePlugService\nlocal simulation" as HardwarePlugService
cloud "SensorServer\nASP.NET Core mock API" as SensorServer

Program --> AppSettings : load settings
Program --> UserSecrets : load API key
Program --> App : resolve and run
App --> ConsoleHost : Host<DashboardController>()
ConsoleHost --> DashboardController : ExecuteAsync(payload)
DashboardController --> DashboardPage : ReturnAsync(model)
DashboardPage --> DashboardRenderer : render dashboard and overlays
DashboardPage --> CommandParser : parse modal text
DashboardController --> CommandExecutor : execute mutation commands
DashboardController --> UniversalApiFacade : fetch sensors heaters fans
CommandExecutor --> UniversalApiFacade : set fan set heater reset preset
UniversalApiFacade --> LocalAdapter : first attempt
UniversalApiFacade --> RemoteAdapter : fallback on local failure
LocalAdapter --> HardwarePlugService : sensor ids 4 to 6 only
RemoteAdapter --> SensorServer : HTTP with API key
LocalAdapter --> ApiResultPipeline : wrap calls
RemoteAdapter --> ApiResultPipeline : wrap calls
LocalAdapter --> ObjectSetterPipeline : build DTOs
RemoteAdapter --> ObjectSetterPipeline : build DTOs
Program --> SerilogQueSink : configure logging sink
Program --> LogFiles : file logging
DashboardRenderer --> SerilogQueSink : GetLogs()
User --> DashboardPage : key input

note bottom of ConsoleHost
The settings parameter exists
The current implementation starts with payload set to null
end note

note bottom of UniversalApiFacade
Dashboard device ids are 1 to 3
LocalAdapter currently succeeds only for sensor ids 4 to 6
end note
@enduml
```

## 2. UI Navigation And Dashboard Interaction Classes

```plantuml
@startuml
title UI navigation and dashboard interaction classes
skinparam classAttributeIconSize 0

interface IRouter {
  +ExecuteAsync(payload)
}

abstract class Controller {
  #Api : IApiService
  #Ok()
  #Ok<T>(value)
  #Fail(error)
  #Fail<T>(error)
  #PayloadAs<TPayload>(payload)
  +ExecuteAsync(payload)
}

class ConsoleHost {
  +Host<TController>(settings)
}

class DashboardController {
  -DeviceCount : int
  -TryExecuteCommandAsync(command)
  -FetchDashboardModelAsync(lastResult)
  +ExecuteAsync(payload)
}

class HomePageController {
  -TestGetUserId(no)
  +ExecuteAsync(payload)
}

interface "IView<TModel>" as IViewT {
  +ReturnAsync(model)
}

class DashboardPage {
  -DispatchKey(key)
  -EnterFanMode()
  -EnterHeaterMode()
  -EnterCommandMode()
  -ConfirmReset()
  -ShowHelpOverlay()
  -ShowLogsOverlay()
  +ReturnAsync(model)
}

class HomePage {
  +ReturnAsync(model)
}

class "MainLayout<TModel>" as MainLayoutT {
  +ReturnAsync(model)
}

interface IDashboardRenderer {
  +RenderDashboard(model)
  +RenderHelp()
  +RenderLogs()
}

class DashboardRenderer

interface ICommandParser {
  +Parse(input, mode)
}

class CommandParser

interface ICommandExecutor {
  +ExecuteAsync(command)
}

class CommandExecutor

interface IDashboardInputReader {
  +ReadKey(intercept)
  +ReadLine()
}

class ConsoleDashboardInputReader

class DashboardModel
class UserDashboardModel
class ParsedCommand
class CommandResult
class NavigationResult
class MenuRouteAttribute

IRouter <|.. Controller
Controller <|-- DashboardController
Controller <|-- HomePageController

ConsoleHost ..> IRouter : resolves controller types

DashboardController --> IViewT : dashboard view
DashboardController --> ICommandExecutor
DashboardController --> DashboardModel
DashboardController --> ParsedCommand
DashboardController --> CommandResult
DashboardController --> NavigationResult

HomePageController --> IViewT : home view
HomePageController --> UserDashboardModel
HomePageController --> NavigationResult

IViewT <|.. DashboardPage
IViewT <|.. HomePage
IViewT <|.. MainLayoutT
MainLayoutT o--> IViewT : decorates inner view

DashboardPage --> IDashboardRenderer
DashboardPage --> ICommandParser
DashboardPage --> IDashboardInputReader
DashboardPage --> ParsedCommand
DashboardPage --> NavigationResult

IDashboardRenderer <|.. DashboardRenderer
ICommandParser <|.. CommandParser
ICommandExecutor <|.. CommandExecutor
IDashboardInputReader <|.. ConsoleDashboardInputReader

DashboardController ..> MenuRouteAttribute
HomePageController ..> MenuRouteAttribute

note bottom of MenuRouteAttribute
Declared on controllers
Not used by the current runtime router
end note

note bottom of HomePageController
Registered in DI and decorated with a layout
App currently starts DashboardController instead
end note
@enduml
```

## 3. Service Layer Results Pipelines And Data Objects

```plantuml
@startuml
title Service layer results pipelines and data objects
skinparam classAttributeIconSize 0

interface IApiService {
  +GetSensorTemperatureAsync(sensorId)
  +GetHeaterDataAsync(heaterId)
  +SetHeaterLevelAsync(heaterId, level)
  +GetFanDataAsync(fanId)
  +SetFanStateAsync(fanId, isOn)
  +ResetAsync()
}

interface IApiPipeline {
  +ExecuteAsync(apiCall)
}

class ApiResultPipeline

interface IObjectSetterPipeline {
  +Execute(func)
}

class ObjectSetterPipeline

abstract class ApiActions {
  +GetAsync<T>(requestUri, createData)
  +SetAsync<TRequest>(requestUri, payload)
  +PostEmptyAsync(requestUri)
}

class UniversalApiFacade
class LocalAdapter
class RemoteAdapter
class HardwarePlugService
class HttpPolicies

class ApiResult {
  +IsSuccess : bool
  +Error : Error
}

class "ApiResult<T>" as ApiResultT

class Error {
  +Code : string
  +Message : string
}

interface IData {
  +Id : int
  +RawJson : string
}

abstract class "StatefulDto<TObject>" as StatefulDtoT {
  +IsModified : bool
  +SaveAction
  +SaveOnChangesAsync()
}

class SensorData {
  +Id : int
  +Temperature : Celcius
  +RawJson : string
}

class HeaterData {
  +Id : int
  +Level : int
  +RawJson : string
}

class FanData {
  +Id : int
  +Status : bool
  +RawJson : string
}

class Celcius {
  +Value : double
  +ToFahrenheit()
}

class TemperaturePreset

IApiService <|.. UniversalApiFacade
IApiService <|.. LocalAdapter
IApiService <|.. RemoteAdapter

ApiActions <|-- LocalAdapter
ApiActions <|-- RemoteAdapter

UniversalApiFacade --> LocalAdapter : try local
UniversalApiFacade --> RemoteAdapter : fallback

IApiPipeline <|.. ApiResultPipeline
IObjectSetterPipeline <|.. ObjectSetterPipeline

LocalAdapter --> ApiResultPipeline
LocalAdapter --> ObjectSetterPipeline
LocalAdapter --> HardwarePlugService

RemoteAdapter --> ApiResultPipeline
RemoteAdapter --> ObjectSetterPipeline
RemoteAdapter ..> HttpPolicies : typed HttpClient retry policy

ApiResultT --|> ApiResult
ApiResult --> Error

IData <|.. SensorData
IData <|.. HeaterData
IData <|.. FanData

StatefulDtoT <|-- SensorData
StatefulDtoT <|-- HeaterData
StatefulDtoT <|-- FanData

SensorData --> Celcius
StatefulDtoT --> ApiResult : save path
TemperaturePreset ..> IApiService : consumed by command layer

note bottom of LocalAdapter
Only sensor reads are implemented
Valid local ids are 4 to 6
All other calls return LocalApi.Fail
end note

note bottom of StatefulDtoT
Failure path is unfinished
Revert throws NotImplementedException
end note
@enduml
```

## 4. Application Lifecycle Activity

```plantuml
@startuml
title Console application lifecycle
start
:Program builds HostApplicationBuilder;
:Load appsettings and user secrets;
:Configure Serilog and typed HttpClient;
:Register controllers pages layouts pipelines and services;
:Build host;
:Resolve App;
:App calls ConsoleHost.Host<DashboardController>;

repeat
  :Resolve current controller from DI;
  :Execute controller with current payload;
  :Controller may execute a command;
  :Controller fetches current snapshot;
  :View renders output and reads next input;
  :Return NavigationResult;
  :Update current route and payload;
repeat while (NextRoute is not null)

stop
@enduml
```

## 5. Refresh Cycle Sequence

```plantuml
@startuml
title Refresh cycle from key press to new dashboard snapshot

actor User
participant ConsoleHost
participant "DashboardController\nprevious cycle" as PrevController
participant DashboardPage
participant DashboardRenderer
participant "DashboardController\nnext cycle" as NextController
participant "UniversalApiFacade\nIApiService" as Api
participant LocalAdapter
participant RemoteAdapter

PrevController -> DashboardPage : ReturnAsync(current model)
DashboardPage -> DashboardRenderer : RenderDashboard(current model)
User -> DashboardPage : press r
DashboardPage -> DashboardPage : DispatchKey()
DashboardPage --> PrevController : NavigationResult(DashboardController, Refresh)
PrevController --> ConsoleHost : NavigationResult(DashboardController, Refresh)

ConsoleHost -> NextController : ExecuteAsync(Refresh)
NextController -> NextController : TryExecuteCommandAsync returns null

par Sensors
  loop sensor id 1 to 3
    NextController -> Api : GetSensorTemperatureAsync(id)
    Api -> LocalAdapter : GetSensorTemperatureAsync(id)
    LocalAdapter --> Api : LocalApi.Fail for ids 1 to 3
    Api -> RemoteAdapter : GetSensorTemperatureAsync(id)
    RemoteAdapter --> Api : ApiResult<SensorData>
    Api --> NextController : ApiResult<SensorData>
  end
else Heaters
  loop heater id 1 to 3
    NextController -> Api : GetHeaterDataAsync(id)
    Api -> LocalAdapter : GetHeaterDataAsync(id)
    LocalAdapter --> Api : LocalApi.Fail
    Api -> RemoteAdapter : GetHeaterDataAsync(id)
    RemoteAdapter --> Api : ApiResult<HeaterData>
    Api --> NextController : ApiResult<HeaterData>
  end
else Fans
  loop fan id 1 to 3
    NextController -> Api : GetFanDataAsync(id)
    Api -> LocalAdapter : GetFanDataAsync(id)
    LocalAdapter --> Api : LocalApi.Fail
    Api -> RemoteAdapter : GetFanDataAsync(id)
    RemoteAdapter --> Api : ApiResult<FanData>
    Api --> NextController : ApiResult<FanData>
  end
end

NextController -> DashboardPage : ReturnAsync(DashboardModel)
DashboardPage -> DashboardRenderer : RenderDashboard(new snapshot)
@enduml
```

## 6. Fan Command Mutation Sequence

```plantuml
@startuml
title Fan command cycle from modal input to feedback bar

actor User
participant DashboardController
participant DashboardPage
participant DashboardRenderer
participant CommandParser
participant CommandExecutor
participant "UniversalApiFacade\nIApiService" as Api
participant LocalAdapter
participant RemoteAdapter

DashboardController -> DashboardPage : ReturnAsync(current snapshot)
DashboardPage -> DashboardRenderer : RenderDashboard(current snapshot)
User -> DashboardPage : press Ctrl+F
DashboardPage -> DashboardPage : EnterFanMode()
User -> DashboardPage : type 1 on
DashboardPage -> CommandParser : Parse("1 on", Fan)
CommandParser --> DashboardPage : ParsedCommand(SetFan, 1, true)
DashboardPage --> DashboardController : NavigationResult(DashboardController, ParsedCommand)

DashboardController -> CommandExecutor : ExecuteAsync(SetFan 1 true)
CommandExecutor -> Api : SetFanStateAsync(1, true)
Api -> LocalAdapter : SetFanStateAsync(1, true)
LocalAdapter --> Api : LocalApi.Fail
Api -> RemoteAdapter : SetFanStateAsync(1, true)
RemoteAdapter --> Api : ApiResult.Success
Api --> CommandExecutor : ApiResult.Success
CommandExecutor --> DashboardController : CommandResult(success, "Fan 1 turned ON.")

note over DashboardController
After the mutation the controller fetches sensors heaters and fans again in parallel
and builds a fresh DashboardModel with feedback attached
end note

DashboardController -> DashboardPage : ReturnAsync(updated snapshot)
DashboardPage -> DashboardRenderer : RenderDashboard(snapshot with feedback bar)
@enduml
```
