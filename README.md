# EasyParsing

C# lite parser combinator helping to create parsers easily.

## Examples

This project contains 2 examples.

- A simple `JSON` parser. 
- A simple `Markdown` parser. 

## Concept of parser combinator

The main goal is to combine multiples small parsers to create a more complex one.

For example, if we want to parse JSON:

```json
{
  "name": "Romain", 
  "age": 39
}
```

We can decompose the problem in multiples steps:
- detecting objects starts `{` (_followed by potentials spaces_)
- parse __quoted string__ for properties (_followed by potentials spaces_)
- parse __value assigment__ character `:` (_followed by potentials spaces_)
- parse __quoted string__ for values (_followed by potentials spaces_)
- handle multiple properties assignements separated by `,` (_followed by potentials spaces_)
- detecting objects ends `}` (_followed by potentials spaces_)

For static methods helpers, add using
```C#
using static EasyParsing.Dsl.Parse;
```

### Objects starts and ends

We can match only one char `{` and __ignore__ all following spaces.  
For this, we can create a small parser.

```C#
IParser<string> StartObject = OneCharText('{') >> SkipSpaces();
```

Operator `>>` will combine both parsers and __ignore__ second result.

For the end, this is the same job:

```C#
IParser<string> EndObject = OneCharText('}') >> SkipSpaces();
```

### Quoted strings

Quoted string parsing can be difficult if we you handle quotes escaping.

`Parse.CreateStringParser(char quoteChar)` can help to create basic quoted strings parsers.

Then `Parse` static class contains

```C#
public static readonly IParser<string> QuotedTextParser = CreateStringParser('\'') | CreateStringParser('"');
```

The operator `|` will tries to run first parser, if first parser fails then it tries to run second.   
So if parser `CreateStringParser('\'')` fails, we try to parse double-quoted string with `CreateStringParser('"')`.


