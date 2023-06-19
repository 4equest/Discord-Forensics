using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CacheViewer
{
    internal class OpenCacheDir
    {
        public static List<MessageData> LoadCacheDir()
        {
            string tmpDir = "tmp";

            if (Directory.Exists(tmpDir))
            {
                DirectoryInfo di = new DirectoryInfo(tmpDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }

            using var cacheDir = new OpenFileDialog
            {
                Title = "Select Cache Directory",
                Filter = "Index|index|(*.*)|*.*",
                AddExtension = false,
                RestoreDirectory = true,
            };

            DialogResult result = cacheDir.ShowDialog();

            if (result == DialogResult.OK && !(string.IsNullOrEmpty(cacheDir.FileName)))
            {
                string dirName = Path.GetDirectoryName(cacheDir.FileName);
                string[] files = Directory.GetFiles(dirName);

                /*
                 * 最低限index data_0 data_1があればファイルをChromeCacheViewで読み込める
                 * 詳細なキャッシュ構造: https://www.chromium.org/developers/design-documents/network-stack/disk-cache/
                 */
                if (files.Contains(Path.Combine(dirName, "index")) && files.Contains(Path.Combine(dirName, "data_0")) && files.Contains(Path.Combine(dirName, "data_1")))
                {
                    ExecuteChromeCacheView(dirName);
                }

                /*
                 * 何らかの理由(実行中のDiscordなど)でindexなどが存在しない場合は全ファイルを調査してjsonを見つけ出す
                 */
                else
                {
                    foreach (string file in files)
                    {
                        if (file.StartsWith("f_"))
                        {
                            LoadCacheFile(file);
                        }
                    }
                }

            }


            return LoadJsonFiles(tmpDir);
        }

        private static List<MessageData> LoadJsonFiles(string dirName)
        {
            string[] files = Directory.GetFiles(dirName);
            List<MessageData> messageDatas = new List<MessageData>();

            foreach (string file in files)
            {
                if (file.EndsWith(".json"))
                {

                    try
                    {
                        using (StreamReader stream = File.OpenText(file))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            dynamic messageData = serializer.Deserialize(stream, typeof(object));
                            foreach (var message in messageData)
                            {
                                MessageData data = new MessageData()
                                {
                                    ID = message.id,
                                    Content = message.content,
                                    ChannelID = message.channel_id,
                                    AuthorID = message.author.id,
                                    AuthorUsername = message.author.username,
                                    Pinned = message.pinned,
                                    Timestamp = message.timestamp,
                                };
                                if (!messageDatas.Any(m => m.ID == data.ID))
                                {
                                    messageDatas.Add(data);
                                }
                            }
                        }
                    }
                    catch (JsonException e)
                    {
                        Console.WriteLine($"Error reading file {file}: {e.Message}");
                    }
                }
            }
            return messageDatas;
        }


        private static void ExecuteChromeCacheView(string dirName)
        {
            ChromeCacheView.ExportCache(dirName);
        }

        private static void LoadCacheFile(string fileName)
        {

        }
    }
}
