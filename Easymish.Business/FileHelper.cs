using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Easymish.Business
{
    public class FileHelper
    {
        public static void SeperateFile(string filePath, string destinateFolder, int fileSize = 512000)
        {
            //string file = @"C:\Users\Ken\Desktop\zetian.txt";
            //string folder = @"C:\Users\Ken\Desktop\1";

            int streamLen = 0;
            byte[] bytes;

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                streamLen = (int)stream.Length;
                bytes = new byte[streamLen];

                stream.Read(bytes, 0, streamLen);

                stream.Close();
            }

            int len = 512000;
            int start = 0;
            int i = 1;

            while (start < streamLen)
            {
                if (start + len > streamLen)
                {
                    len = streamLen - start;
                }
                string path = Path.Combine(destinateFolder, i.ToString() + ".txt");

                using (FileStream toStream = new FileStream(path, FileMode.Create))
                {
                    toStream.Write(bytes, start, len);
                }

                start = start + len;
                i++;
            }
        }
    }
}
