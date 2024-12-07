
# EasyParsing.Samples.Json

Small sample for JSON parsing with EasyParsing.

As a string variable `json` containing:

```json
{
  "name": "Joe John",
  "description": "Hi, I'm \"cool\" !!",
  "age": 39,
  "hobbies": [
    "lol",
    76,
    {
      "id": 1234,
      "score": 1223.234,
      "names": ["game 1", "game 2"]
    }
  ]
}
```

Then you can convert the `JSON` to AST with:

```C#
var result = JsonParser.JsonArrayParser.Parse(json);
```
Returned value will be equivalent to

```C#
new JsonObject(new Dictionary<string, JsonValue>
{
    { "name", new JsonStringValue("Joe John") },
    { "description", new JsonStringValue("Hi, I'm \"cool\" !!") },
    { "age", new JsonLongValue(39) },
    { "hobbies", 
        new JsonArray([
            new JsonStringValue("lol"),
            new JsonLongValue(76),
            
            new JsonObject(new Dictionary<string, JsonValue>
            {
                {"id", new JsonStringValue("1234")},
                {"score", new JsonDecimalValue(1223.234m)},
                {
                    "names", 
                    new JsonArray([
                        new JsonStringValue("game 1"),
                        new JsonStringValue("game 2"),
                    ])
                },
            })
            
        ])
    }
})
```
