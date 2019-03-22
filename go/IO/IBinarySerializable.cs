// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;

namespace go
{
    /// <summary>
    ///  Classes implement IBinarySerializable for fast binary serialization.
    /// </summary>
    public interface IBinarySerializable
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}
