using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CSAudioConverter;
using NVorbis;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;

namespace BeatSaberSongArchiver
{
    public partial class Form1 : Form
    {
		private List<ConvertTask> convertTasks = new List<ConvertTask>();

		private string[] audioFiles;
		private int audioFileIndex = 0;
		private int processedAudioFiles = 0;
		private long savedSpaceAudio = 0;

		private int processedCoverFiles = 0;
		private long savedSpaceCover = 0;

		public Form1()
        {
			InitializeComponent();

			audioBitrate.SelectedItem = "192 Kbps";
			coverSize.SelectedItem = "512x512 px";
        }

		private void btnLocate_Click(object sender, EventArgs e)
        {
			using (var bsFolderDialog = new FolderBrowserDialog())
			{
				if (bsFolderDialog.ShowDialog() != DialogResult.OK)
					return;

				txtBsLocation.Text = bsFolderDialog.SelectedPath;
			}
		}

        private void Form1_Load(object sender, EventArgs e)
        {
			try
			{
				var p = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 620980").GetValue("InstallLocation");

				if (p != null && CheckFolderPath((string)p))
					txtBsLocation.Text = (string)p;
			}
			catch { }
		}

		bool CheckFolderPath(string path)
		{
			if (!Directory.Exists(path))
				return false;

			if (!File.Exists(Path.Combine(path, "..", "..", "appmanifest_620980.acf")))
				return false;

			return true;
		}

		private void btnApply_Click(object sender, EventArgs e)
        {
			StartConversion();
		}

		private void StartConversion()
        {
			string bsFolderLocation = txtBsLocation.Text;

			progressBar.Value = 0;

			if (bsFolderLocation == String.Empty)
			{
				MessageBox.Show("Please select a Beat Saber install folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				string customLevelsDir = bsFolderLocation + @"\Beat Saber_Data\CustomLevels";

				if (!Directory.Exists(customLevelsDir))
				{
					MessageBox.Show("Can't find CustomLevels folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					EnableForm(false);

					var checkedAudio = audioBitrate.Text;
					var checkedCover = coverSize.Text;

					int targetAudioBitrate = 0;
					int targetCoverSize = 0;

					audioFiles = Directory.GetFiles(customLevelsDir, "*.egg", SearchOption.AllDirectories);

					if (checkedAudio != "No compression")
						targetAudioBitrate = int.Parse(checkedAudio.Split(' ')[0]);

					if (checkedCover != "No compression")
						targetCoverSize = int.Parse(checkedCover.Split('x')[0]);

					if (audioFiles.Length > 0)
                    {
						foreach (string audioFile in audioFiles)
						{
							convertTasks.Add(new ConvertTask(audioFile));
						}

						var maxThreads = 16;
						var options = new ParallelOptions() { MaxDegreeOfParallelism = maxThreads };
						Parallel.ForEach(convertTasks, options, convertTask =>
						{
							ConvertFile(convertTask, targetAudioBitrate, targetCoverSize);
						});

						EnableForm(true);

						progressBar.Value = 100;

						MessageBox.Show($"{processedAudioFiles} songs compressed: {SizeSuffix(savedSpaceAudio)} saved.\n{processedCoverFiles} covers resized: {SizeSuffix(savedSpaceCover)} saved.", "Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);

						audioFileIndex = 0;
						processedAudioFiles = 0;
						savedSpaceAudio = 0;

						processedCoverFiles = 0;
						savedSpaceCover = 0;
					}
				}
			}
		}

		private void ConvertFile(ConvertTask convertTask, int bitrate, int coverSize)
        {
			FileInfo file = new FileInfo(convertTask.sourceFile);

			// Cover resize
			if (coverSize > 0)
			{
				string coverPath = file.DirectoryName;
				string[] coverFilesJPG = Directory.GetFiles(coverPath, "*.jpg");
				string[] coverFilesJPEG = Directory.GetFiles(coverPath, "*.jpeg");
				string[] coverFilesPNG = Directory.GetFiles(coverPath, "*.png");

				string[] coverFiles = coverFilesJPG.Concat(coverFilesJPEG).Concat(coverFilesPNG).ToArray();

				foreach (string cover in coverFiles)
				{
					FileInfo coverFile = new FileInfo(cover);
					Image img = Image.FromFile(cover);
					int originalWidth = img.Width;

					if (originalWidth > coverSize)
					{
						string targetCoverFileName = coverFile.Directory.FullName + "\\converted_" + coverFile.Name;

						Bitmap imgbitmap = new Bitmap(img);
						PixelFormat pf = img.PixelFormat.ToString().Contains("Indexed") ? PixelFormat.Format24bppRgb : img.PixelFormat;
						Image resizedImage = resizeImage(imgbitmap, coverSize, coverSize, pf);

						if (coverFile.Extension == ".jpg" || coverFile.Extension == ".jpeg")
                        {
							ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
							EncoderParameters encoderParameters = new EncoderParameters(1);
							encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 70L);
							resizedImage.Save(targetCoverFileName, jpgEncoder, encoderParameters);
						}
						else
                        {
							resizedImage.Save(targetCoverFileName, img.RawFormat);
						}

						long resizedSize = new FileInfo(targetCoverFileName).Length;

						savedSpaceCover += coverFile.Length - resizedSize;
						processedCoverFiles++;

						img.Dispose();
						imgbitmap.Dispose();
						resizedImage.Dispose();

						File.Delete(cover);
						File.Move(targetCoverFileName, cover);
					}
				}
			}

			// Audio conversion
			if (bitrate > 0)
			{
				// OGG infos
				convertTask.vorbis = new VorbisReader(convertTask.sourceFile);

				var channels = convertTask.vorbis.Channels;
				var sampleRate = convertTask.vorbis.SampleRate;
				var nominalBitrate = convertTask.vorbis.NominalBitrate / 1000;

				convertTask.vorbis.Dispose();

				if (bitrate < nominalBitrate)
				{
					Debug.WriteLine(convertTask.sourceFile);

					convertTask.audioConverter = new AudioConverter();

					convertTask.destinationFile = file.Directory.FullName + "\\converted_" + file.Name;

					// Output Bitrate
					Bitrate outputBitrate = (Bitrate)Enum.Parse(typeof(Bitrate), "bitrate" + bitrate);

					// Output Channels
					Channels outputChannels = (Channels)Enum.Parse(typeof(Channels), "channels" + channels);

					// Output Samplerate
					Samplerate outputSamplerate = Samplerate.fsamples48000;

					switch (sampleRate)
					{
						case 8000:
							outputSamplerate = Samplerate.asamples8000;
							break;
						case 11025:
							outputSamplerate = Samplerate.bsamples11025;
							break;
						case 22050:
							outputSamplerate = Samplerate.csamples22050;
							break;
						case 44100:
							outputSamplerate = Samplerate.esamples44100;
							break;
					}

					// Conversion

					// Licence
					convertTask.audioConverter.UserName = "Free@Usage";
					convertTask.audioConverter.UserKey = "d1200cee3a2f9f7aeddb37f9ea398592";

					// Settings
					convertTask.audioConverter.Format = Format.OGG;
					convertTask.audioConverter.DestinatioFile = convertTask.destinationFile;
					convertTask.audioConverter.DecodeMode = DecodeMode.FFMpeg;
					convertTask.audioConverter.Bitrate = outputBitrate;
					convertTask.audioConverter.Channels = outputChannels;
					convertTask.audioConverter.Samplerate = outputSamplerate;

					convertTask.audioConverter.SourceFiles.Clear();
					convertTask.audioConverter.SourceFiles.Add(new Options.Core.SourceFile(convertTask.sourceFile));

					convertTask.audioConverter.Convert();

					convertTask.audioConverter.ConvertDone += (s, e) =>
					{
						convertTask.finished = true;

						FileInfo sourceFile = new FileInfo(convertTask.sourceFile);
						FileInfo convertedFile = new FileInfo(convertTask.destinationFile);
						savedSpaceAudio += sourceFile.Length - convertedFile.Length;
						processedAudioFiles++;

						File.Delete(convertTask.sourceFile);
						File.Move(convertTask.destinationFile, convertTask.sourceFile);
					};

					// Wait
					while (convertTask.finished != true)
					{
						Thread.Sleep(200);
					}
				}
			}
		}

		private void ConvertNext()
        {
			if (audioFileIndex + 1 < audioFiles.Length)
			{
				audioFileIndex++;
				progressBar.Value = audioFileIndex * 100 / audioFiles.Length;

				// ConvertFile(audioFiles[audioFileIndex]);
			}
			else
			{
				EnableForm(true);

				progressBar.Value = 100;

				MessageBox.Show($"{processedAudioFiles} songs compressed: {SizeSuffix(savedSpaceAudio)} saved.\n{processedCoverFiles} covers resized: {SizeSuffix(savedSpaceCover)} saved.", "Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);

				audioFileIndex = 0;
				processedAudioFiles = 0;
				savedSpaceAudio = 0;

				processedCoverFiles = 0;
				savedSpaceCover = 0;
			}
		}

		private Image resizeImage(Image image, int width, int height, PixelFormat pixelFormat)
		{
			var destinationRect = new Rectangle(0, 0, width, height);
			var destinationImage = new Bitmap(width, height, pixelFormat);

			destinationImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destinationImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighSpeed;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destinationRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return (Image)destinationImage;
		}

		private ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}
			return null;
		}

		private void EnableForm(bool enabled)
		{
			ControlBox = enabled;
			txtBsLocation.Enabled = enabled;
			btnLocate.Enabled = enabled;
			audioSettings.Enabled = enabled;
			coverSettings.Enabled = enabled;
			btnArchive.Enabled = enabled;
		}

		static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

		static string SizeSuffix(Int64 value)
		{
			if (value < 0) { return SizeSuffix(-value); }

			int i = 0;
			decimal dValue = (decimal)value;
			while (Math.Round(dValue / 1024) >= 1)
			{
				dValue /= 1024;
				i++;
			}

			return string.Format("{0:n1} {1}", dValue, SizeSuffixes[i]);
		}
	}
}
