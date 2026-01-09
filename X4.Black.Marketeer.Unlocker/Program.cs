// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.IO.Compression;
using System.Xml.Linq;

/// <summary>
/// Provides the entry point and core logic for the application that unlocks all black marketeers
/// in a specified X4 save file.
/// </summary>
internal sealed class Program
{
	/// <summary>
	/// Serves as the entry point for the application, unlocking all black marketeers in the specified X4 save file.
	/// </summary>
	/// <remarks>
	/// The method expects a single argument specifying the path to the X4 save file. It creates a backup
	/// of the file if one does not already exist, unlocks all black marketeers by modifying the save file,
	/// and saves the changes. If the required argument is missing, usage information is displayed and the
	/// application exits.
	/// </remarks>
	/// <param name="args">An array of command-line arguments.
	/// The first element must be the path to the X4 save file to process.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	private static async Task Main(string[] args)
	{
		CancellationTokenSource cts = new();
		CancellationToken ct = cts.Token;

		if (args.Length != 1)
		{
			Console.WriteLine("Usage: X4.Black.Marketeer.Unlocker <path to X4 save file>");
			return;
		}

		string saveFilePath = args[0];
		string backupPath = args[0] + ".bak";

		if (!File.Exists(saveFilePath))
		{
			Console.WriteLine($"Error: Save file '{saveFilePath}' does not exist.");
			return;
		}

		Console.WriteLine("Creating backup of the save file...");
		File.Copy(saveFilePath, backupPath, true);

		XElement savegame = await LoadSaveGameFile(saveFilePath, ct)
			.ConfigureAwait(false);

		UnlockAllBlackMarketeers(savegame);

		await SaveSaveGameFile(savegame, saveFilePath, ct)
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Unlocks all black marketeer components in the specified savegame by making their trades visible.
	/// </summary>
	/// <remarks>
	/// This method updates the 'flags' attribute of each black marketeer's 'traits' element to include
	/// 'tradesvisible', if not already present. Only components with a 'stockid' attribute value of
	/// 'default_shadyguy' are affected.
	/// </remarks>
	/// <param name="savegame">The XML element representing the savegame data to modify.</param>
	private static void UnlockAllBlackMarketeers(XElement savegame)
	{
		IEnumerable<XElement> marketeers = savegame.Descendants("component")
			.Where(x => x.Attribute("stockid")?.Value == "default_shadyguy");

		Console.WriteLine($"Found {marketeers.Count()} black marketeers.");

		foreach (XElement marketeer in marketeers)
		{
			string faction = marketeer.Attribute("owner")?.Value ?? "unknown";
			string name = marketeer.Attribute("name")?.Value ?? "unknown";
			string code = marketeer.Attribute("code")?.Value ?? "unknown";

			string? flags = marketeer.Element("traits")?.Attribute("flags")?.Value;

			if (flags is null)
				continue;

			if (flags.Contains("tradesvisible"))
			{
				Console.WriteLine($"Marketeer '{name}' ({code}) of faction '{faction}' is already unlocked.");
				continue;
			}

			marketeer.Element("traits")?.SetAttributeValue("flags", flags + "|tradesvisible");
			Console.WriteLine($"Marketeer '{name}' ({code}) of faction '{faction}' has been unlocked.");
		}
	}

	/// <summary>
	/// Asynchronously saves the specified save game data to a compressed file at the given path.
	/// </summary>
	/// <remarks>
	/// The save game data is written in a compressed format. If a file already exists at the specified
	/// path, it will be overwritten.
	/// </remarks>
	/// <param name="savegame">The XML element containing the save game data to be written to the file.</param>
	/// <param name="saveFilePath">The full file path where the compressed save game file will be created or overwritten.</param>
	/// <param name="ct">A cancellation token that can be used to cancel the save operation.</param>
	/// <returns>A task that represents the asynchronous save operation.</returns>
	private static async Task SaveSaveGameFile(XElement savegame, string saveFilePath, CancellationToken ct)
	{
		Console.WriteLine("Saving modified save file...");

		using MemoryStream memoryStream = new();

		await savegame.SaveAsync(memoryStream, SaveOptions.DisableFormatting, ct)
			.ConfigureAwait(false);

		memoryStream.Position = 0;

		await CompressToFileAsync(memoryStream, saveFilePath, ct)
			.ConfigureAwait(false);

		memoryStream.Close();

		Console.WriteLine("Save file saved.");
	}

	/// <summary>
	/// Asynchronously loads and parses a save game file from the specified path, returning its
	/// contents as an XML element.
	/// </summary>
	/// <remarks>
	/// The method decompresses the save game file before parsing it as XML. If the file is not
	/// a valid or supported save game file, an exception may be thrown during decompression or XML parsing.
	/// </remarks>
	/// <param name="saveFilePath">The full path to the save game file to load. The file must exist and be accessible.</param>
	/// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains an <see cref="XElement"/>
	/// representing the root element of the loaded save game file.
	/// </returns>
	private static async Task<XElement> LoadSaveGameFile(string saveFilePath, CancellationToken ct)
	{
		Console.WriteLine("Loading save file...");

		using MemoryStream decompressedStream = await DecompressFromFileAsync(saveFilePath, ct)
			.ConfigureAwait(false);

		XElement element = await XElement.LoadAsync(decompressedStream, LoadOptions.None, ct)
			.ConfigureAwait(false);

		decompressedStream.Close();

		Console.WriteLine("Save file loaded.");

		return element;
	}

	/// <summary>
	/// Asynchronously decompresses a GZip-compressed file and returns its contents as a memory stream.
	/// </summary>
	/// <remarks>
	/// The caller is responsible for disposing the returned memory stream when it is no longer needed.
	/// If the file specified by inputFile does not exist or is not a valid GZip file, an exception will
	/// be thrown.
	/// </remarks>
	/// <param name="inputFile">The path to the GZip-compressed file to decompress. Must refer to an existing file.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
	/// <returns>
	/// A task that represents the asynchronous operation. The task result contains a <see cref="MemoryStream"/>.
	/// </returns>
	private static async Task<MemoryStream> DecompressFromFileAsync(string inputFile, CancellationToken cancellationToken)
	{
		Console.WriteLine("Decompressing save file...");

		MemoryStream memoryStream = new();

		using (FileStream fileStream = File.Open(inputFile, FileMode.Open, FileAccess.Read, FileShare.Read))
		{
			using GZipStream gzipStream = new(fileStream, CompressionMode.Decompress);

			await gzipStream.CopyToAsync(memoryStream, cancellationToken)
				.ConfigureAwait(false);

			gzipStream.Close();
			fileStream.Close();
		}

		memoryStream.Position = 0;

		Console.WriteLine("Decompression complete.");

		return memoryStream;
	}

	/// <summary>
	/// Asynchronously compresses the contents of the specified input stream and writes the compressed
	/// data to a file in GZip format.
	/// </summary>
	/// <remarks>
	/// The method does not close the input stream after compression. The output file will be created or
	/// overwritten. The operation may be partially complete if the cancellation token is triggered before
	/// completion.
	/// </remarks>
	/// <param name="inputStream">The stream containing the data to compress. The stream must be readable
	/// and positioned at the start of the data to be compressed.</param>
	/// <param name="outputFile">The path of the file to which the compressed data will be written.
	/// If the file exists, it will be overwritten.</param>
	/// <param name="cancellationToken">A cancellation token that can be used to cancel the
	/// asynchronous operation.</param>
	/// <returns>A task that represents the asynchronous compression operation.</returns>
	private static async Task CompressToFileAsync(Stream inputStream, string outputFile, CancellationToken cancellationToken)
	{
		Console.WriteLine("Compressing save file...");

		using FileStream outputStream = File.Create(outputFile);
		using GZipStream gzip = new(outputStream, CompressionMode.Compress);

		await inputStream.CopyToAsync(gzip, cancellationToken);

		gzip.Close();
		outputStream.Close();

		Console.WriteLine("Compression complete.");
	}
}
