using EasyParsing.Markdown.Ast;

namespace EasyParsing.Markdown.Html;

/// <summary>
/// Provides functionality to convert Markdown text into HTML.
/// </summary>
public sealed class MarkdownToHtmlConverter
{
    private readonly Dictionary<Type, Func<MarkdownAst, StreamWriter, MarkdownAstWriter, Task>> defaultWriters = new();

    public MarkdownToHtmlConverter()
    {
        DefaultMarkdownAstWriters.RegisterWriters(defaultWriters);
    }

    /// <summary>
    /// Registers a writer function for converting a specific type of Markdown AST node to HTML.
    /// </summary>
    /// <typeparam name="T">The type of the Markdown AST node to be handled by the writer.</typeparam>
    /// <param name="writerFunc">A function that writes the specified type of Markdown AST node to a stream writer asynchronously.</param>
    public void RegisterWriter<T>(Func<T, StreamWriter, MarkdownAstWriter, Task> writerFunc) where T : MarkdownAst
    {
        defaultWriters[typeof(T)] = (ast, streamWriter, writer) =>
        {
            var typedAst = (T)ast;
            return writerFunc.Invoke(typedAst, streamWriter, writer);
        };
    }
    
    /// <summary>
    /// Converts the given markdown text into HTML and writes the result to the specified output stream.
    /// </summary>
    /// <param name="markdown">The markdown text to convert.</param>
    /// <param name="outputStream">The stream to write the converted HTML content to (not disposed at the end).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ConvertAsync(string markdown, Stream outputStream)
    {
        var ast = MarkdownParser.ParseMarkdown(markdown);
        
        await using var streamWriter = new StreamWriter(outputStream);

        foreach (var node in ast)
        {
            await Write(node, streamWriter);
        }
        
        await streamWriter.FlushAsync();
        await outputStream.FlushAsync();
    }

    /// <summary>
    /// Writes a Markdown AST node to the specified stream writer asynchronously.
    /// </summary>
    /// <param name="node">The Markdown AST node to be written.</param>
    /// <param name="streamWriter">The writer that outputs the resulting content.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no writer is registered for the node's type.</exception>
    private async Task Write(MarkdownAst node, StreamWriter streamWriter)
    {
        if (!defaultWriters.TryGetValue(node.GetType(), out var writerFunc))
            throw new InvalidOperationException($"No writer for node type {node.GetType().Name}");
            
        await writerFunc.Invoke(node, streamWriter, Write);
            
        await streamWriter.FlushAsync();
    }
}