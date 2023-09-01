﻿using Intuit.Ipp.OAuth2PlatformClient;
using System.Text.Json;

namespace QboLib
{
    public class QboLocal
    {
        public static QboAuthTokens? Tokens { get; set; } = null;
        public static OAuth2Client? Client { get; set; } = null;

        public static void Initialize()
        {
            // Loading the tokens and client once (on sign-in/start up)
            // and saving them in static properties saves us from
            // deserializing again when we want to read or write the data.
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string tokenFilePath = Path.Combine(directory, "Merlin", "Tokens.jsonc");
            
            Tokens = JsonSerializer.Deserialize<QboAuthTokens>(File.ReadAllText(tokenFilePath), new JsonSerializerOptions()
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            }) ?? new();

            // In the case that the data failed to deserialize, the ClientId
            // and ClientSecret will be null, we need to make sure that's
            // handled correctly.
            if (!string.IsNullOrEmpty(Tokens.ClientId) && !string.IsNullOrEmpty(Tokens.ClientSecret))
            {
                Client = new(Tokens.ClientId, Tokens.ClientSecret, Tokens.RedirectUrl, Tokens.Environment);
            }
            else
            {
                throw new InvalidDataException(
                    "The ClientId or ClientSecret was null or empty.\n" +
                    "Make sure that 'Tokens.jsonc' is setup with your credentials."
                );
            }
        }
    }
}
