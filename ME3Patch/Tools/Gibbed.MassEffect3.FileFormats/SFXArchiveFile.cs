/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace Gibbed.MassEffect3.FileFormats
{
    public class SFXArchiveFile
    {
        public Endian Endian;
        public List<uint> BlockSizes
            = new List<uint>();
        public List<SFXArchive.Entry> Entries
            = new List<SFXArchive.Entry>();
        public uint MaximumBlockSize;
        public SFXArchive.CompressionScheme CompressionScheme;

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream input)
        {
            var magic = input.ReadValueU32(Endian.Little);
            if (magic != 0x53464152 && // SFAR
                magic.Swap() != 0x53464152)
            {
                throw new FormatException();
            }
            var endian = magic == 0x53464152 ? Endian.Little : Endian.Big;

            var version = input.ReadValueU32(endian);
            if (version != 0x00010000)
            {
                throw new FormatException();
            }

            var dataOffset = input.ReadValueU32(endian);
			bool firstDataOffset = true;
			uint minDataOffset = dataOffset;
            //Console.WriteLine("Data Offset: {0:X8}",dataOffset);
            var fileTableOffset = input.ReadValueU32(endian);
            //Console.WriteLine("File Table Offset: {0:X8}",fileTableOffset);
            var fileTableCount = input.ReadValueU32(endian);
            //Console.WriteLine("File Table Count: {0:X8}",fileTableCount);
            var blockSizeTableOffset = input.ReadValueU32(endian);
            //Console.WriteLine("Block Size Table Offset: {0:X8}",blockSizeTableOffset);
            this.MaximumBlockSize = input.ReadValueU32(endian);
            this.CompressionScheme = input
                .ReadValueEnum<SFXArchive.CompressionScheme>(endian);

            if (fileTableOffset != 0x20)
            {
                throw new FormatException();
            }

            if (this.MaximumBlockSize != 0x010000)
            {
                throw new FormatException();
            }

            /*
            if (this.CompressionScheme != SFXArchive.CompressionScheme.None &&
                this.CompressionScheme != SFXArchive.CompressionScheme.LZMA &&
                this.CompressionScheme != SFXArchive.CompressionScheme.LZX)
            {
                throw new FormatException();
            }
            */
            input.Seek(fileTableOffset, SeekOrigin.Begin);
            for (uint i = 0; i < fileTableCount; i++)
            {
// ReSharper disable UseObjectOrCollectionInitializer
                var entry = new SFXArchive.Entry();
				entry.entryOffset = input.Position;
// ReSharper restore UseObjectOrCollectionInitializer
                entry.nameHash = input.ReadFileNameHash();
            	//Console.WriteLine("FileNameHash: {0}",entry.NameHash.ToString());
                entry.blockSizeIndex = input.ReadValueS32(endian);
            	//Console.WriteLine("Begin position: {0:X8}",input.Position);
                entry.uncompressedSize = input.ReadValueU32(endian);
                entry.uncompressedSize |= ((long)input.ReadValueU8()) << 32;
            	//Console.WriteLine("  End position: {0:X8}",input.Position);
                entry.dataOffset = input.ReadValueU32(endian);
                entry.dataOffset |= ((long)input.ReadValueU8()) << 32;
				if(firstDataOffset)
				{
					minDataOffset = (uint)entry.dataOffset;
					firstDataOffset = false;
				}
				else
				{
					if(minDataOffset > entry.dataOffset)
						minDataOffset = (uint)entry.dataOffset;
				}
				//if(entry.NameHash.Equals (fileNameListNameHash))Console.WriteLine("Offset: {0:X10}, UncSize {1:X10}",entry.Offset,entry.UncompressedSize);
                this.Entries.Add(entry);
            }
			if(minDataOffset > dataOffset)
				dataOffset = minDataOffset;

            input.Seek(blockSizeTableOffset, SeekOrigin.Begin);

            var blockSizeTableSize = dataOffset - blockSizeTableOffset;
            var blockSizeTableCount = blockSizeTableSize / 2;
			//ushort aux;
            //Console.WriteLine("dataOffset: {0:X8}\nfileTableOffset: {1:X8}\nBlockSizeTableSize: {2:X8}\nblockSizeTableOffset: {3:X8}", dataOffset,fileTableOffset,blockSizeTableSize,blockSizeTableOffset);
            this.BlockSizes.Clear();
            //Console.WriteLine("initial position: {0:X8}",input.Position);
            //Console.WriteLine("blockSizeTableCount: {0}",blockSizeTableCount);
            for (uint i = 0; i < blockSizeTableCount; i++)
            {
                this.BlockSizes.Add(input.ReadValueU16(endian));
            }
            //Console.WriteLine("final position: {0:X8}",input.Position);
            //Console.WriteLine("number of repetitions: {0}",blockSizeTableCount);
			//var fileNameListNameHash = new FileNameHash(
            //        new byte[] { 0xB5, 0x50, 0x19, 0xCB, 0xF9, 0xD3, 0xDA, 0x65, 0xD5, 0x5B, 0x32, 0x1C, 0x00, 0x19, 0x69, 0x7C, });

        }
    }
}
