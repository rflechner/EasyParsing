using EasyParsing.Markdown.Ast;

namespace EasyParsing.Markdown.Html;

/// <summary>
/// Provides a registry for default Markdown abstract syntax tree (AST) writers,
/// which handle the conversion of specific AST elements to their formatted output representation.
/// </summary>
public static class DefaultMarkdownAstWriters
{
    /// <summary>
    /// Adapts a strongly-typed function for writing a specific Markdown abstract syntax tree (AST) node to a generalized function that handles any Markdown AST node.
    /// </summary>
    /// <typeparam name="T">The specific type of Markdown AST node to be written.</typeparam>
    /// <param name="func">A strongly-typed function for writing a specific Markdown AST node asynchronously.</param>
    /// <returns>A generalized function for writing Markdown AST nodes asynchronously, wrapping the input function for the specified type.</returns>
    private static Func<MarkdownAst, StreamWriter, MarkdownAstWriter, Task> Wrap<T>(Func<T, StreamWriter, MarkdownAstWriter, Task> func)
        where T : MarkdownAst
    {
        return (ast, streamWriter, writer) => func((T)ast, streamWriter, writer);
    }
    
    /// <summary>
    /// Registers functions for writing specific types of Markdown abstract syntax tree (AST) nodes to their HTML representation within the given registry.
    /// </summary>
    /// <param name="registry">A dictionary that maps Markdown AST node types to functions capable of writing their corresponding HTML output asynchronously.</param>
    public static void RegisterWriters(IDictionary<Type, Func<MarkdownAst, StreamWriter, MarkdownAstWriter, Task>> registry)
    {
        foreach (var writer in GetWriters())
        {
            registry[writer.Method.GetParameters()[0].ParameterType] = writer;
        }
    }
    
    /// <summary>
    /// Retrieves the default set of functions for writing Markdown abstract syntax tree (AST) elements to a stream writer asynchronously.
    /// </summary>
    /// <returns>An enumerable collection of delegate functions, each responsible for handling a specific type of Markdown AST node.</returns>
    public static IEnumerable<Func<MarkdownAst, StreamWriter, MarkdownAstWriter, Task>> GetWriters()
    {
        yield return Wrap<Bold>(BoldWriter);
        yield return Wrap<Crlf>(CrlfWriter);
        yield return Wrap<Image>(ImageWriter);
        yield return Wrap<InlineQuotingCode>(InlineQuotingCodeWriter);
        yield return Wrap<Italic>(ItalicWriter);
        yield return Wrap<Link>(LinkWriter);
        yield return Wrap<ListItem>(ListItemWriter);
        yield return Wrap<ListItems>(ListItemsWriter);
        yield return Wrap<ParagraphStart>(ParagraphStartWriter);
        yield return Wrap<QuotingCode>(QuotingCodeWriter);
        yield return Wrap<QuotingText>(QuotingTextWriter);
        yield return Wrap<Strikethrough>(StrikethroughWriter);
        yield return Wrap<TaskListItem>(TaskListItemWriter);
        yield return Wrap<Title>(TitleWriter);
    }
    
    /// <summary>
    /// Converts a bold Markdown abstract syntax tree (AST) node into an HTML representation and writes it asynchronously to a stream.
    /// </summary>
    /// <param name="bold">The bold Markdown AST node to be converted into HTML.</param>
    /// <param name="streamWriter">The stream writer to which the HTML representation will be written.</param>
    /// <param name="writer">The delegate for writing nested Markdown AST nodes to the stream writer asynchronously.</param>
    /// <returns>A task that represents the asynchronous writing operation.</returns>
    public static async Task BoldWriter(Bold bold, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteAsync("<b>");

        foreach (var ast in bold.Content) await writer(ast, streamWriter);
            
        await streamWriter.WriteAsync("</b>");
    }

    /// <summary>
    /// Writes an HTML representation of a carriage return line feed (CRLF) Markdown abstract syntax tree (AST) node to a stream writer asynchronously.
    /// </summary>
    /// <param name="crlf">The CRLF Markdown AST node to be written.</param>
    /// <param name="streamWriter">The stream writer used to output the HTML representation of the CRLF node.</param>
    /// <param name="writer">The Markdown AST writer function delegate for additional nested AST nodes (if applicable).</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task CrlfWriter(Crlf crlf, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteLineAsync();
        await streamWriter.WriteAsync("<br />");
        await streamWriter.WriteLineAsync();
    }

    /// <summary>
    /// Writes an image Markdown abstract syntax tree (AST) element as an HTML image tag asynchronously.
    /// </summary>
    /// <param name="image">The image AST node containing the title and URL for the HTML image tag.</param>
    /// <param name="streamWriter">The stream writer used to write the HTML output.</param>
    /// <param name="writer">The delegate for writing child AST nodes if necessary.</param>
    /// <returns>A task that represents the asynchronous writing operation.</returns>
    public static Task ImageWriter(Image image, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        return streamWriter.WriteLineAsync($"<img src=\"{image.Url}\" alt=\"{image.Title}\" >");
    }

    /// <summary>
    /// Writes an inline quoting code block to the specified stream writer, formatting it as an HTML pre element.
    /// </summary>
    /// <param name="code">The inline quoting code segment to be written, represented by text enclosed in backticks or similar syntax.</param>
    /// <param name="streamWriter">The stream writer to which the formatted HTML representation of the inline code will be written.</param>
    /// <param name="writer">The delegate responsible for handling nested or related Markdown AST elements during output conversion.</param>
    /// <returns>A task representing the asynchronous operation of writing the formatted inline quoting code to the stream.</returns>
    public static Task InlineQuotingCodeWriter(InlineQuotingCode code, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        return streamWriter.WriteLineAsync($"<span class=\"inline-code\">{code.Text}</span>");
    }

    /// <summary>
    /// Handles the writing of an italicized Markdown abstract syntax tree (AST) element to a stream asynchronously.
    /// </summary>
    /// <param name="italic">The <see cref="Italic"/> AST node representing the italicized text to be written.</param>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> to which the formatted text is written.</param>
    /// <param name="writer">The function for writing nested or child Markdown AST nodes to the stream.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
    public static async Task ItalicWriter(Italic italic, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteAsync("<i>");

        foreach (var ast in italic.Content) await writer(ast, streamWriter);
            
        await streamWriter.WriteAsync("</i>");
    }

    /// <summary>
    /// Writes a Markdown hyperlink to the specified stream in HTML format asynchronously.
    /// </summary>
    /// <param name="link">The <see cref="Link"/> node representing the hyperlink to write to the output stream.</param>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> used to write the hyperlink output.</param>
    /// <param name="writer">The delegate used to write the inner text of the hyperlink recursively.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task LinkWriter(Link link, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteAsync($"<a href=\"{link.Url}\" title=\"{link.Title}\">");

        await writer(link.Text, streamWriter);
        
        await streamWriter.WriteAsync("</a>");
    }

    /// <summary>
    /// Writes a collection of list items in Markdown AST to a stream as HTML unordered list elements.
    /// </summary>
    /// <param name="listItems">The collection of list items to be written.</param>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> to write the output to.</param>
    /// <param name="writer">The generalized Markdown AST writer to handle child nodes within the list items.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task ListItemsWriter(ListItems listItems, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteLineAsync("<ul>");

        foreach (var item in listItems.Items) await ListItemWriter(item, streamWriter, writer);
        
        await streamWriter.WriteLineAsync("</ul>");
    }

    /// <summary>
    /// Writes an individual Markdown list item, including its content and any nested list items, to the provided stream writer.
    /// </summary>
    /// <param name="listItem">The list item to be written, representing a Markdown list element with potential nested content.</param>
    /// <param name="streamWriter">The stream writer to output the formatted list item content.</param>
    /// <param name="writer">A delegate for writing Markdown AST nodes, used to handle the processing of nested list contents.</param>
    /// <returns>A task that represents the asynchronous operation of writing a list item and its nested content.</returns>
    public static async Task ListItemWriter(ListItem listItem, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteAsync("<li>");

        foreach (var ast in listItem.Content) await writer(ast, streamWriter);

        if (listItem.NestedList.Count > 0)
        {
            foreach (var item in listItem.NestedList) await ListItemWriter(item, streamWriter, writer);
        }
        
        await streamWriter.WriteAsync("</li>");
    }

    /// <summary>
    /// Writes the opening representation of a paragraph for Markdown to the provided stream writer asynchronously.
    /// </summary>
    /// <param name="_">The <see cref="ParagraphStart"/> node representing the start of a Markdown paragraph.</param>
    /// <param name="streamWriter">The <see cref="StreamWriter"/> to which the output will be written.</param>
    /// <param name="writer">A delegate for writing Markdown abstract syntax tree (AST) nodes.</param>
    /// <returns>A task that represents the asynchronous writing operation.</returns>
    public static Task ParagraphStartWriter(ParagraphStart _, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        return streamWriter.WriteLineAsync("<div style=\"line-height: 1; height: 1em;\"><div>");
    }

    /// <summary>
    /// Writes a Markdown abstract syntax tree (AST) node representing a block of quoted code to an output stream in HTML format.
    /// </summary>
    /// <param name="code">The AST node representing a block of quoted code, containing its text and optional programming language.</param>
    /// <param name="streamWriter">The stream writer used to write the HTML output.</param>
    /// <param name="writer">A delegate for writing additional child nodes or related content.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static Task QuotingCodeWriter(QuotingCode code, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        return streamWriter.WriteLineAsync($"<pre class=\"code\">{code.Text}</pre>");
    }

    /// <summary>
    /// Writes a quoted text Markdown abstract syntax tree (AST) element to an output stream in HTML format.
    /// </summary>
    /// <param name="text">The quoted text AST element to be written.</param>
    /// <param name="streamWriter">The stream writer for outputting the formatted HTML.</param>
    /// <param name="writer">A generalized function for handling Markdown AST nodes.</param>
    /// <returns>A task that represents the asynchronous operation of writing the quoted text element.</returns>
    public static Task QuotingTextWriter(QuotingText text, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        return streamWriter.WriteLineAsync($"<pre class=\"quoted-text\">{text.Text}</pre>");
    }

    /// <summary>
    /// Handles the writing of a Strikethrough Markdown abstract syntax tree (AST) node to a specified output stream.
    /// </summary>
    /// <param name="strikethrough">The Strikethrough AST node to be written. Contains the content to render with a strikethrough style.</param>
    /// <param name="streamWriter">The StreamWriter used to write the resulting output content.</param>
    /// <param name="writer">The generalized Markdown AST writer function used to process child nodes within the Strikethrough content.</param>
    /// <returns>A task representing the asynchronous operation of writing the Strikethrough node to the output stream.</returns>
    public static async Task StrikethroughWriter(Strikethrough strikethrough, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteAsync("<div class=\"strikethrough\">");

        foreach (var ast in strikethrough.Content) await writer(ast, streamWriter);
            
        await streamWriter.WriteAsync("</div>");
    }

    /// <summary>
    /// Writes a TaskListItem Markdown AST node to the output stream in an HTML representation.
    /// </summary>
    /// <param name="taskListItem">The TaskListItem node containing the task item's state (checked or unchecked) and its content.</param>
    /// <param name="streamWriter">The StreamWriter used to write the HTML output to the stream.</param>
    /// <param name="writer">A delegated MarkdownAstWriter function for processing and writing nested Markdown AST nodes.</param>
    /// <returns>A Task representing the asynchronous operation of writing the TaskListItem node to the stream.</returns>
    public static async Task TaskListItemWriter(TaskListItem taskListItem, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        await streamWriter.WriteLineAsync("<br />");

        await streamWriter.WriteLineAsync("<input type=\"checkbox\" ");
        
        if (taskListItem.Checked)
            await streamWriter.WriteLineAsync("checked ");
            
        await streamWriter.WriteLineAsync("/>");
        
        foreach (var ast in taskListItem.Content) await writer(ast, streamWriter);
            
        await streamWriter.WriteLineAsync("<br />");
    }

    /// <summary>
    /// Writes a Title Markdown AST node to a stream asynchronously, generating HTML output based on its depth and content.
    /// </summary>
    /// <param name="title">The Title AST node to be written, containing depth and nested content.</param>
    /// <param name="streamWriter">The stream writer to which the HTML output for the Title node will be written.</param>
    /// <param name="writer">A delegate used to write nested Markdown AST nodes contained within the Title node.</param>
    /// <returns>A task representing the asynchronous operation of writing the Title node's content to the stream.</returns>
    public static async Task TitleWriter(Title title, StreamWriter streamWriter, MarkdownAstWriter writer)
    {
        var titleDepth = Math.Max(1, title.Depth);
        await streamWriter.WriteAsync($"<h{titleDepth}>");
        foreach (var ast in title.Content) await writer(ast, streamWriter);
        await streamWriter.WriteAsync($"</h{titleDepth}>");
    }
}