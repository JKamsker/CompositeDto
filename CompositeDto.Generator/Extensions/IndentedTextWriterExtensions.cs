using System.CodeDom.Compiler;
using System.IO;

namespace CompositeDto.Generator.Helpers;

public static class IndentedTextWriterExtensions
{
    public static void WriteLine(
        this IndentedTextWriter writer,
        int offset,
        string value
    )
    {
        _ = writer ?? throw new System.ArgumentNullException(nameof(writer));

        writer.Indent += offset;
        writer.WriteLine(value);
        writer.Indent -= offset;
    }

    public static void WriteLines(
        this IndentedTextWriter writer,
        string[] lines
    )
    {
        _ = writer ?? throw new System.ArgumentNullException(nameof(writer));
        _ = lines ?? throw new System.ArgumentNullException(nameof(lines));

        foreach (var line in lines)
        {
            writer.WriteLine(line);
        }
    }

    public static void AppendOpenBracket(
        this IndentedTextWriter writer
    )
    {
        _ = writer ?? throw new System.ArgumentNullException(nameof(writer));

        writer.WriteLine("{");
        writer.Indent++;
    }

    public static void AppendCloseBracket(
        this IndentedTextWriter writer
    )
    {
        _ = writer ?? throw new System.ArgumentNullException(nameof(writer));

        writer.Indent--;
        writer.WriteLine("}");
    }

    public static void UnwindOpenedBrackets(
        this IndentedTextWriter writer
    )
    {
        _ = writer ?? throw new System.ArgumentNullException(nameof(writer));

        while (writer.Indent != 0)
        {
            AppendCloseBracket(writer);
        }
    }

    // Writes xmldoc comment with the specified value. Value can be multiline. Each line should start with ``///``.
    public static void WriteXmlDocComment(
        this IndentedTextWriter writer,
        string? value
    )
    {
        if (writer is null || string.IsNullOrWhiteSpace(value))
        {
            return;
        }


        var lines = value.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        // writer.WriteLine("/// <summary>");
        foreach (var line in lines)
        {
            writer.WriteLine($"/// {line}");
        }

        // writer.WriteLine("/// </summary>");
    }
}