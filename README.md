# Библиотека для работы с REST API

Эта библиотека разработана на C# и предназначена для взаимодействия с RESTful API.

## Основные функции

- **Инициализация API-сервиса:** Создание экземпляра ApiService для отправки запросов к удаленному API.

- **Выполнение запросов:** Отправка GET-запросов к удаленному API с использованием заданных параметров и моделей запросов.

- **Обработка ответов:** Получение и десериализация ответов от удаленного API для дальнейшей обработки.

## Пример использования

Вот пример использования программы для работы с REST API:

```csharp
using GiRest;
using Newtonsoft.Json;

// Создание экземпляра ApiService для взаимодействия с удаленным API
ApiService apiService = new ApiService("https://deckofcardsapi.com/api");

// Выполнение GET-запроса для создания новой колоды и получение ответа
var newDeckResponse = apiService.DoRequest<NewDeckResponse>("/deck/new/shuffle/", "GET", request: new NewDeckRequest() { deck_count = 1 });
var deckId = newDeckResponse.DeckId;
Console.WriteLine(deckId);

// Выполнение GET-запроса для получения карт из колоды
var drawResponse = apiService.DoRequest<DrawResponse>($"/deck/{deckId}/draw/", "GET", request: new DrawRequest() { Count = 4 });
Console.WriteLine(string.Join("\n", drawResponse.Cards.Select(c => c.Code + ": " + c.Image)));
```

## Модели запросов и ответов
Программа использует модели запросов и ответов для взаимодействия с удаленным API. Вот примеры таких моделей:

### Модель запроса для создания новой колоды

```csharp
class NewDeckRequest
{
    [PropertyName("deck_count")]
    public int deck_count { get; set; }
}
```

### Модель ответа для создания новой колоды

```csharp
class NewDeckResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("deck_id")]
    public string DeckId { get; set; }

    // Другие поля ответа...
}
```
### Модель запроса для получения карт из колоды

```csharp
public class DrawRequest
{
    [PropertyName("count")]
    public int Count { get; set; }
}
```

### Модель ответа для получения карт из колоды

```csharp
public class DrawResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("deck_id")]
    public string DeckId { get; set; }

    [JsonProperty("cards")]
    public List<Card> Cards { get; set; }

    // Другие поля ответа...

    public class Card
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        // Другие поля карты...
    }
}
```

## Зависимости
Программа зависит от библиотеки GiRest и Newtonsoft.Json для работы с REST API и JSON-сериализацией.

## Лицензия
Программа распространяется под лицензией MIT.

## Связь
Если у вас возникли вопросы или проблемы с использованием этой программы для работы с REST API, не стесняйтесь связаться со мной по адресу gigster2000@yandex.ru.
