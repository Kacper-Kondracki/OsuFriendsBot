using Discord;
using Discord.Commands;

namespace OsuFriendsBot.Embeds
{
    public class CommandsEmbed : EmbedBuilder
    {
        public CommandsEmbed(ModuleInfo module, string prefix)
        {
            Title = module.Name;
            Description = module.Summary ?? "No summary";
            Color = EmbedColors.Info;
            foreach (CommandInfo command in module.Commands)
            {
                string title = string.Join(" | ", command.Aliases);
                if (command.Summary != null)
                {
                    title += $" | {command.Summary}";
                }
                string value = $"{prefix}{command.Name}";
                foreach (ParameterInfo parameter in command.Parameters)
                {
                    value += $" | {parameter.Name}";
                    if (parameter.Summary != null)
                    {
                        value += $": {parameter.Summary}";
                    }
                }
                value = Format.Code($"\n{value}", "css");
                AddField(title, value);
            }
        }
    }
}