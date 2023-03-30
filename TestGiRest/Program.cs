using GiRest;
using Newtonsoft.Json;

ApiService apiService = new ApiService("https://deckofcardsapi.com/api");
var newDeckResponse = ApiService.DoRequest<NewDeckResponse>("/deck/new/shuffle/", "GET", request: new NewDeckRequest() { deck_count = 1 });
var deckId = newDeckResponse.DeckId;
Console.WriteLine(deckId);

var drawResponse = apiService.DoRequest<DrawResponse>($"/deck/{deckId}/draw/", "GET", request: new DrawRequest() { Count = 4 });
Console.WriteLine(string.Join("\n", drawResponse.Cards.Select(c => c.Code + ": " + c.Image)));

class NewDeckRequest
{
    [PropertyName("deck_count")]
    public int deck_count { get; set; }
}

class NewDeckResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("deck_id")]
    public string DeckId { get; set; }

    [JsonProperty("shuffled")]
    public bool Shuffled { get; set; }

    [JsonProperty("remaining")]
    public int Remaining { get; set; }
}

public class DrawRequest
{
    [PropertyName("count")]
    public int Count { get; set; }
}

public class DrawResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("deck_id")]
    public string DeckId { get; set; }

    [JsonProperty("cards")]
    public List<Card> Cards { get; set; }

    public class Card
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("images")]
        public Images CardImages { get; set; }

        public class Images
        {
            [JsonProperty("svg")]
            public string Svg { get; set; }

            [JsonProperty("png")]
            public string Png { get; set; }
        }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("suit")]
        public string Suit { get; set; }
    }

    [JsonProperty("remaining")]
    public int Remaining { get; set; }
}