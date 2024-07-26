using TwitchLib.Client.Events;

namespace TwitchService.Handlers.Commands;

public class HelpCommand(List<Command> commands) : Command("help")
{
    private readonly List<Command> _commands = commands;
    public override void Execute(OnChatCommandReceivedArgs e, ChatHandler chatHandler)
    {

    }
}