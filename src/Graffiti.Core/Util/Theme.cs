using System;
using System.Collections.Generic;
using System.IO;

namespace Graffiti.Core
{
	[Serializable]
	public class Theme
	{
		private string _baseDirectory = string.Empty;
		private List<ThemeFile> _files = new List<ThemeFile>();
		private string _name = string.Empty;

		public Theme()
		{
		}

		public Theme(string baseThemeDirectory, string themeName, bool loadFiles)
			: this()
		{
			_name = themeName;
			_baseDirectory = baseThemeDirectory;

			if (loadFiles)
				LoadFiles();
		}

		public Theme(string baseThemeDirectory, string themeName)
			: this(baseThemeDirectory, themeName, true)
		{
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public List<ThemeFile> Files
		{
			get { return _files; }
		}

		public void AddFile(string file)
		{
			ThemeFile newFile = new ThemeFile(this, file);

			Files.Add(newFile);
		}

		private void LoadFiles()
		{
			string themeDir = _baseDirectory + _name;
			var files = Directory.GetFiles(themeDir);


			foreach (string file in files)
			{
				var fileParts = file.Split(Path.DirectorySeparatorChar);
				string fileName = fileParts[fileParts.Length - 1];

				if (fileName.Contains(".") &&
				    (fileName == "theme.config" || fileName.Substring(fileName.LastIndexOf(".") + 1) == "css" ||
				     fileName.Substring(fileName.LastIndexOf(".") + 1) == "view"))
				{
					AddFile(file);
				}
			}
		}
	}
}