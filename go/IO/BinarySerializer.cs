// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;

namespace go.IO
{
    public static class BinarySerializer
    {
        public static void LoadFromFile(IBinarySerializable instance, string filePath)
        {
            using (BinaryReader r = new BinaryReader(File.OpenRead(filePath)))
            {
                instance.Read(r);
            }
        }

        public static void SaveToFile(IBinarySerializable instance, string filePath)
        {
            string fullPath = Path.GetFullPath(filePath);
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (BinaryWriter w = new BinaryWriter(File.OpenWrite(filePath)))
            {
                instance.Write(w);
            }
        }
    }
}
