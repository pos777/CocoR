using System;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CocoR.Generator {

public sealed class ConsoleOutInterceptor : IDisposable {
	private readonly TaskLoggingHelper log;
	private readonly TextWriter consoleOut;
	private readonly StringBuilder buffer;
	private readonly TextWriter output;

	public ConsoleOutInterceptor(TaskLoggingHelper log) {
		this.log = log;
		consoleOut = Console.Out;
		buffer = new StringBuilder();
		output = new StringWriter(buffer);
		Console.SetOut(output);
	}

	void IDisposable.Dispose() {
		Console.SetOut(consoleOut);
		output.Dispose();
		using (var reader = new StringReader(buffer.ToString())) {
			log.LogMessagesFromStream(reader, MessageImportance.High);
		}
	}
}

}
