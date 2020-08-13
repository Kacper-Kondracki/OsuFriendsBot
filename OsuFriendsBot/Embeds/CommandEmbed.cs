using Discord;
using Discord.Commands;

namespace OsuFriendsBot.Embeds
{
    public class CommandEmbed : EmbedBuilder
    {
        public CommandEmbed(CommandInfo command, string prefix)
        {
            Title = $"{prefix}{command.Name}";
            Description = command.Summary ?? "No summary";
            Color = EmbedColors.Info;

            foreach (ParameterInfo parameter in command.Parameters)
            {
                Title += $" {parameter.Name}";
                string title = parameter.Name;
                if (parameter.Summary != null)
                {
                    title += $" ({parameter.Summary})";
                }
                string value = $"Type: {parameter.Type} | Optional: {(parameter.IsOptional ? "Yes" : "No")}";
                value = Format.Code($"\n{value}", "css");
                AddField(title, value);
            }
        }
    }
}