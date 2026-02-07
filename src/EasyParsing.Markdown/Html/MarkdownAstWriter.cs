using EasyParsing.Markdown.Ast;

namespace EasyParsing.Markdown.Html;

public delegate Task MarkdownAstWriter(MarkdownAst node, StreamWriter writer);