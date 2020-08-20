# Bot Dispatch Runner

The Bot Dispatch runner is an example how to use the [Bot Dispatch CLI](https://github.com/microsoft/botbuilder-tools/tree/master/packages/Dispatch) in a programmatic way.

### Features
- Automatic retrieval of the Dispatch CLI tool with npm
- References the closed source Dll's from the example project
- Runs the dispatch tool as new process
- supports runtime timeout
- supports async/await

## Installation

- clone the directory
- dotnet build the solution

## Usage

### Console Application
- check [BotDispatch.Runner](./BotDispatch.Runner)
- added all references with hints in the .csproj

### Azure Function
- check [BotDispatch.Function](./BotDispatch.Function)
- added all references in the csproj too, and additional path to the json


```csharp
var workingDirectory = Path.Join(Path.GetTempPath(), "dispatch");
var dispatchRunner = new DispatchRunner(workingDirectory);
var result = await dispatchRunner.RunDispatchAsync("-h");
```

## Implementation details
`BotDispatch.NPM` has an additional build step to call npm and restore botdispatch npm package. As the target environment might not have node.js installed, the Dlls are copied to the output of the referenced project. 

## License
[MIT](https://choosealicense.com/licenses/mit/)