using System.Collections.Generic;
using System.IO;
using System.Linq;
using at.jku.ssw.Coco;
using CocoR.Generator;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace MSBuildTask {

public sealed class GenerateCocoR : Task {

	[Required]
	public ITaskItem[] GrammarFiles { get; set; }

	[Output]
	public ITaskItem[] OutputFiles { get; private set; }

	public override bool Execute() {
		if (GrammarFiles != null) {
			bool hasErrors = false;
			var outputFiles = new List<string>();
			foreach (var grammarFile in GrammarFiles) {
				string srcName = grammarFile.ItemSpec;
				string nsName = GetMetadata(grammarFile, "Namespace");
				string frameDir = GetMetadata(grammarFile, "FrameFilesDirectory");
				string ddtString = GetMetadata(grammarFile, "TraceString");
				string outDir = GetMetadata(grammarFile, "OutputDirectory");
				string emitLinesStr = GetMetadata(grammarFile, "Lines");
				bool.TryParse(emitLinesStr, out bool emitLines);
				using (new ConsoleOutInterceptor(Log)) {
					if (Coco.Generate(srcName, nsName, frameDir, ddtString, outDir, emitLines) == 0) {
						string dir = outDir != null ? outDir : Path.GetDirectoryName(srcName);
						outputFiles.Add(Path.Combine(dir, "Scanner.cs"));
						outputFiles.Add(Path.Combine(dir, "Parser.cs"));
					} else {
						hasErrors = true;
					}
				}
			}

			OutputFiles = outputFiles.Select(file => new TaskItem(file)).ToArray();
			return !hasErrors && !Log.HasLoggedErrors;
		}

		return !Log.HasLoggedErrors;
	}

	private static string GetMetadata(ITaskItem taskItem, string metadataName) {
		string result = taskItem.GetMetadata(metadataName);
		return result != "" ? result : null;
	}
}

}
