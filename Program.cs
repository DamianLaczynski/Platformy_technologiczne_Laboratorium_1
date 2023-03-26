using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            showDirectoryContent(args[0], 0);
            showOldestElement(args[0]);
            serializeDirectory(args[0]);
            Console.WriteLine();
            deserializeDirectory();

            Console.ReadKey();
        }

        static void serializeDirectory(string dirPath)
        {
            FileStream fs = new FileStream("DirInfo.txt", FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, loadToCollection(dirPath));
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed serializable. Message: "+ e.Message);
            }
            finally
            {
                
                fs.Close();
            }
        }

        static void deserializeDirectory()
        {
            SortedList<string, long> list = null;

            FileStream fs = new FileStream("DirInfo.txt", FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                list = (SortedList<string, long>)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            foreach(var item in list)
            {
                Console.WriteLine("{0} -> {1}", item.Key, item.Value);
            }

        }

        static void showOldestElement(string dirPath)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dirPath);
            Console.WriteLine("Najstarszy plik: " + dirInfo.oldestElement());
        }

        static void showDirectoryContent(string dirPath, uint level)
        {

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dirPath);

            //pobranie wszystkich katalogów podrzędnych i wyświelenie ich
            System.IO.DirectoryInfo[] dirsInfo = dirInfo.GetDirectories();
            System.IO.FileInfo[] filesInfo = dirInfo.GetFiles("*.*");

            for (int i = 0; i < level; i++)
            {
                Console.Write("\t");
            }

            Console.WriteLine("{0} ({1}) " + dirInfo.getDOSAttributes(), dirInfo.Name, filesInfo.Length + dirsInfo.Length);

            foreach (System.IO.DirectoryInfo fi in dirsInfo)
            {
                showDirectoryContent(fi.FullName, level + 1);
            }
            
            foreach (System.IO.FileInfo fi in filesInfo)
            {
                for (int i = 0; i < level+1; i++)
                {
                    Console.Write("\t");
                }
                Console.WriteLine("{0} {1} bajtow " + fi.getDOSAttributes(), fi.Name, fi.Length);
            }
        }

        static SortedList<string, long> loadToCollection(string dirPath)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(dirPath);
            

            SortedList<string, long> list = new SortedList<string, long>(new namesComparer());
            System.IO.DirectoryInfo[] dirsInfo = dirInfo.GetDirectories();
            System.IO.FileInfo[] filesInfo = dirInfo.GetFiles("*.*");
            foreach (System.IO.DirectoryInfo fi in dirsInfo)
            {
                System.IO.DirectoryInfo[] fiDirInfo = fi.GetDirectories();
                System.IO.FileInfo[] fiFilesInfo = fi.GetFiles("*.*");
                list.Add(fi.Name, fiDirInfo.Length + fiFilesInfo.Length);   
            }
            foreach(System.IO.FileInfo fi in filesInfo)
            {
                list.Add(fi.Name, fi.Length);
            }
            return list;
        }

    }
    public static class MyExtension
    {
        static DateTime oldest = DateTime.Now;
        public static DateTime oldestElement(this DirectoryInfo directoryInfo)
        {

            System.IO.DirectoryInfo[] dirsInfo = directoryInfo.GetDirectories();
            foreach (System.IO.DirectoryInfo fi in dirsInfo)
            {

                fi.oldestElement();
            }

            System.IO.FileInfo[] filesInfo = directoryInfo.GetFiles("*.*");

            foreach (System.IO.FileInfo fi in filesInfo)
            {
                if (fi.CreationTime < oldest)
                {
                    oldest = fi.CreationTime;
                }
            }

            return oldest;
        }

        public static string getDOSAttributes(this FileSystemInfo fileSystemInfo)
        {
            string result = "";
            FileAttributes atr = fileSystemInfo.Attributes;
            if ((atr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                result += "r";
            else
                result += "-";

            if ((atr & FileAttributes.Archive) == FileAttributes.Archive)
                result += "a";
            else
                result += "-";

            if ((atr & FileAttributes.Hidden) == FileAttributes.Hidden)
                result += "h";
            else
                result += "-";

            if ((atr & FileAttributes.System) == FileAttributes.System)
                result += "s";
            else
                result += "-";
            return result;
        }
    }

    [Serializable]
    public class namesComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if(a.Length > b.Length)
            {
                return 1;
            }
            else if(a.Length == b.Length)
            {
                for(int i = 0; i < a.Length; i++)
                {
                    if (a[i] > b[i])
                        return 1;
                    else if (a[i] == b[i])
                        continue;
                    else
                        return -1;
                }
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

}
