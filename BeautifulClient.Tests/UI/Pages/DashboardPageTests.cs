using BeautifulClient.Data.Enums;
using BeautifulClient.Data.Records;
using BeautifulClient.UI.Commands;
using BeautifulClient.UI.Controllers;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Pages;
using BeautifulClient.UI.Rendering;
using Spectre.Console;

namespace BeautifulClient.Tests.UI.Pages;

public class DashboardPageTests
{
    [Fact]
    public async Task ReturnAsync_ReturnsQuitCommand_WhenQIsPressed()
    {
        var renderer = new StubRenderer();
        var parser = new StubParser();
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, false));
        var subject = new DashboardPage(renderer, parser, input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.Quit, command.Type);
        Assert.Equal(1, renderer.DashboardRenderCalls);
    }

    [Fact]
    public async Task ReturnAsync_CtrlF_EntersFanModeAndParsesInput()
    {
        var renderer = new StubRenderer();
        var parser = new StubParser
        {
            ParseResult = new ParsedCommand(DashboardCommandType.SetFan, DeviceId: 1, Value: true)
        };
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.F, false, false, true));
        input.EnqueueLine("1 on");
        var subject = new DashboardPage(renderer, parser, input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.SetFan, command.Type);
        Assert.Equal("1 on", parser.LastInput);
        Assert.Equal(DashboardInputMode.Fan, parser.LastMode);
    }

    [Fact]
    public async Task ReturnAsync_CtrlH_EntersHeaterModeAndParsesInput()
    {
        var parser = new StubParser
        {
            ParseResult = new ParsedCommand(DashboardCommandType.SetHeater, DeviceId: 2, Value: 3)
        };
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.H, false, false, true));
        input.EnqueueLine("2 3");
        var subject = new DashboardPage(new StubRenderer(), parser, input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.SetHeater, command.Type);
        Assert.Equal("2 3", parser.LastInput);
        Assert.Equal(DashboardInputMode.Heater, parser.LastMode);
    }

    [Fact]
    public async Task ReturnAsync_CtrlA_EntersCommandModeAndParsesInput()
    {
        var parser = new StubParser
        {
            ParseResult = new ParsedCommand(DashboardCommandType.ApplyPreset, Value: "warm")
        };
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.A, false, false, true));
        input.EnqueueLine("preset warm");
        var subject = new DashboardPage(new StubRenderer(), parser, input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.ApplyPreset, command.Type);
        Assert.Equal(DashboardInputMode.Command, parser.LastMode);
    }

    [Fact]
    public async Task ReturnAsync_QuestionMark_ShowsHelpAndReturnsRefresh()
    {
        var renderer = new StubRenderer();
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('?', ConsoleKey.Oem2, false, false, false));
        input.EnqueueKey(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        var subject = new DashboardPage(renderer, new StubParser(), input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.Refresh, command.Type);
        Assert.Equal(1, renderer.HelpRenderCalls);
    }

    [Fact]
    public async Task ReturnAsync_CtrlL_ShowsLogsAndReturnsRefresh()
    {
        var renderer = new StubRenderer();
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.L, false, false, true));
        input.EnqueueKey(new ConsoleKeyInfo('x', ConsoleKey.X, false, false, false));
        var subject = new DashboardPage(renderer, new StubParser(), input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.Refresh, command.Type);
        Assert.Equal(1, renderer.LogsRenderCalls);
    }

    [Fact]
    public async Task ReturnAsync_CtrlRWithYes_ReturnsResetCommand()
    {
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.R, false, false, true));
        input.EnqueueKey(new ConsoleKeyInfo('y', ConsoleKey.Y, false, false, false));
        var subject = new DashboardPage(new StubRenderer(), new StubParser(), input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.Reset, command.Type);
    }

    [Fact]
    public async Task ReturnAsync_CtrlRWithNo_ReturnsRefreshCommand()
    {
        var input = new QueueInputReader();
        input.EnqueueKey(new ConsoleKeyInfo('\0', ConsoleKey.R, false, false, true));
        input.EnqueueKey(new ConsoleKeyInfo('n', ConsoleKey.N, false, false, false));
        var subject = new DashboardPage(new StubRenderer(), new StubParser(), input);

        AnsiConsole.Record();
        NavigationResult result = await subject.ReturnAsync(new DashboardModel());

        ParsedCommand command = AssertPayload(result);
        Assert.Equal(DashboardCommandType.Refresh, command.Type);
    }

    private static ParsedCommand AssertPayload(NavigationResult result)
    {
        Assert.Equal(typeof(DashboardController), result.NextRoute);
        return Assert.IsType<ParsedCommand>(result.Payload);
    }

    private sealed class StubRenderer : IDashboardRenderer
    {
        public int DashboardRenderCalls { get; private set; }
        public int HelpRenderCalls { get; private set; }
        public int LogsRenderCalls { get; private set; }

        public void RenderDashboard(DashboardModel model) => DashboardRenderCalls++;

        public void RenderHelp() => HelpRenderCalls++;

        public void RenderLogs() => LogsRenderCalls++;
    }

    private sealed class StubParser : ICommandParser
    {
        public string LastInput { get; private set; } = string.Empty;
        public DashboardInputMode LastMode { get; private set; }
        public ParsedCommand ParseResult { get; set; } = new(DashboardCommandType.Refresh);

        public ParsedCommand Parse(string input, DashboardInputMode mode = DashboardInputMode.Command)
        {
            LastInput = input;
            LastMode = mode;
            return ParseResult;
        }
    }

    private sealed class QueueInputReader : IDashboardInputReader
    {
        private readonly Queue<ConsoleKeyInfo> keyQueue = new();
        private readonly Queue<string?> lineQueue = new();

        public void EnqueueKey(ConsoleKeyInfo key) => keyQueue.Enqueue(key);

        public void EnqueueLine(string? line) => lineQueue.Enqueue(line);

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            if (keyQueue.Count == 0)
            {
                throw new InvalidOperationException("No queued key input.");
            }

            return keyQueue.Dequeue();
        }

        public string? ReadLine()
        {
            if (lineQueue.Count == 0)
            {
                throw new InvalidOperationException("No queued line input.");
            }

            return lineQueue.Dequeue();
        }
    }
}
