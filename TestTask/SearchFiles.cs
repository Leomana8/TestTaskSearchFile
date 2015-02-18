using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestTask
{
    class SearchFiles
    {
        private int _countFiles;
        public delegate void FileFoundDelegate(FileInfo file);
        public event FileFoundDelegate FileFound;
        public delegate void AnyFileFoundDelegate(FileInfo file);
        public event FileFoundDelegate AnyFileFound;

        public int CountFiles
        {
            get { return _countFiles; }
        }
        public SearchFiles()
        {
            _countFiles = 0;
        }

        public async Task Find(Term term, CancellationToken cancellationToken)
        {
            _countFiles = 0;
            DirectoryInfo root = new DirectoryInfo(term.StartFolder);
            string pattern = term.Pattern;
            if (term.Pattern == "")
                pattern = "*";
            foreach (var file in NextFile(root, pattern))
            {
                AnyFileFound(file);
                await Task.Run(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Task.FromResult(false);
                        return;
                    }
                });
                if (file != null)
                {                    
                    if (await FindText(file, term.Text, cancellationToken))
                    {
                        // найден файл с заданным текстом
                        FileFound(file);
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
        }

        private IEnumerable<FileInfo> NextFile(DirectoryInfo path, string pattern)
        {
            FileInfo[] files = null;
            try
            {
                files = path.GetFiles(pattern, SearchOption.TopDirectoryOnly);
            }
            catch (UnauthorizedAccessException)
            {
                // запрет в доступе
            }
            foreach (var file in files)
            {
                _countFiles++;
                yield return file;
            }
            if (files.Length == 0)
                yield return null;
            // поиск в подкаталогах
            foreach (var dir in path.GetDirectories())
            {
                foreach (var file in NextFile(dir, pattern))
                {
                   yield return file;
                }
            }
        }

        private async Task<bool> FindText(FileInfo file, string text, CancellationToken cancellationToken)
        {
            bool res = false;
            await Task.Run(() =>
                {
                    try
                        {
                    res =
                        (from line in File.ReadLines(file.FullName)
                         where line.Contains(text) && !cancellationToken.IsCancellationRequested
                         select line
                     ).Any();
                    }
                    catch (IOException)
                    {
                    }
                });
            return res;                  
        }
    }
}
