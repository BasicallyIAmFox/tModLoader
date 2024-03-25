using System;
using System.CodeDom.Compiler;
using System.IO;

namespace tModLoader.SourceGenerator;

public sealed class IndentedStringBuilder : IndentedTextWriter
{
	public readonly struct BlockScope(IndentedStringBuilder writer) : IDisposable
	{
		public readonly void Dispose()
		{
			writer.Indent--;
			writer.WriteLine('}');
		}
	}

	public IndentedStringBuilder() : base(new StringWriter(), "\t")
	{
	}

	public sealed override string ToString()
	{
		var stringWriter = (StringWriter)InnerWriter;
		var stringBuilder = stringWriter.GetStringBuilder();
		stringWriter.Close();
		return stringBuilder.ToString();
	}

	public BlockScope WriteBlock()
	{
		WriteLine('{');
		Indent++;

		return new BlockScope(this);
	}
}
