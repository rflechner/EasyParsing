
# EasyParsing.Samples.Markdown

Small sample for Markdown parsing with EasyParsing.

As a string variable `md` containing:

```markdown
# Big title 1

Hello, does it work ?

## Second title **`with bold code`**

- list item 1
  + lol
  + cool !
- list item 2: __with bold content__

```

Then you can convert the `markdown` to AST with:

```C#
var success = MarkdownParser.TryParseMarkdown(markdown, out var ast)
```

If parsing fails, then `success` will be false.

Otherwise `ast` will have a value like

```C#
[
    new Crlf(),
    new Title(1, [new RawText("Big title 1")]),
    new ParagraphStart(),
    new RawText("Hello, does it work ?"),
    new ParagraphStart(),
    new Title(2, [
        new RawText("Second title"),
        new Bold([
            new InlineQuotingCode("with bold code")
        ])
    ]),
    new ParagraphStart(),
    new ListItems([
        new ListItem(0, "-", [new RawText("list item 1")], 
        [
            new ListItem(2, "+", [new RawText("lol")], []),
            new ListItem(2, "+", [new RawText("cool !")], [])
        ]),
        new ListItem(0, "-", [new RawText("list item 2")], [])
    ])
]
```
