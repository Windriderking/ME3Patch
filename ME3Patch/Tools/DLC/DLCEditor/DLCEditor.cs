#define WITH_GUI

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Gibbed.IO;
using Gibbed.MassEffect3.FileFormats;

#if (WITH_GUI)
using System.ComponentModel;
#endif

namespace AmaroK86.MassEffect3
{
    public class DLCEditor
    {
        public enum action
        {
            copy,
            add,
            replace,
            delete
        };

        protected class add
        {
            public add(string fName, string fPath)
            {
                fileName = fName;
                filePath = fPath;
            }
            public string fileName;
            public string filePath;
        };

        public bool verbose = false;
        private DLCBase dlcBase = null;

        SortedDictionary<FileNameHash, add> listAdd;
        SortedDictionary<FileNameHash, string> listReplace;
        SortedDictionary<FileNameHash, string> listDelete;
        public SortedDictionary<FileNameHash, action> listComplete;

        int blockSizeCount;

        public DLCEditor(DLCBase dlcBase)
        {
            this.dlcBase = dlcBase;
            listAdd = new SortedDictionary<FileNameHash, add>();
            listReplace = new SortedDictionary<FileNameHash, string>();
            listDelete = new SortedDictionary<FileNameHash, string>();
            listComplete = new SortedDictionary<FileNameHash, action>();
            blockSizeCount = (int)(this.dlcBase.dataOffset - this.dlcBase.blockTableOffset) / 2;

            foreach (sfarFile entry in dlcBase.fileList)
            {
                listComplete.Add(entry.nameHash, action.copy);
            }
            //listComplete.Add(DLCBase.fileListHash, action.copy);
        }

        private bool fileIsPresent(string fileName)
        {
            foreach (sfarFile entry in dlcBase.fileList)
            {
                if (entry.fileName == fileName)
                    return true;
            }
            return false;
        }

        public void setAddFile(string newFilePathName, string fileName)
        {
            if (!fileIsPresent(newFilePathName))
            {
                listAdd.Add(FileNameHash.Compute(newFilePathName), new add(newFilePathName, fileName));
                listComplete.Add(FileNameHash.Compute(newFilePathName), action.add);
            }
        }

        public void undoAddFile(string newFilePathName)
        {
            listAdd.Remove(FileNameHash.Compute(newFilePathName));
            listComplete.Remove(FileNameHash.Compute(newFilePathName));
        }

        public void setReplaceFile(string fileToReplace, string newFile)
        {
            if (fileIsPresent(fileToReplace))
            {
                listReplace.Add(FileNameHash.Compute(fileToReplace), newFile);
                listComplete[FileNameHash.Compute(fileToReplace)] = action.replace;
            }
        }

        public void undoReplaceFile(string fileToReplace)
        {
            listReplace.Remove(FileNameHash.Compute(fileToReplace));
            listComplete[FileNameHash.Compute(fileToReplace)] = action.copy;
        }

        public void setDeleteFile(string fileName)
        {
            if (fileIsPresent(fileName))
            {
                listDelete.Add(FileNameHash.Compute(fileName), fileName);
                listComplete[FileNameHash.Compute(fileName)] = action.delete;
            }
        }

        public void undoDeleteFile(string fileName)
        {
            listDelete.Remove(FileNameHash.Compute(fileName));
            listComplete[FileNameHash.Compute(fileName)] = action.copy;
        }

        public bool checkForExec()
        {
            return (listAdd.Count > 0) || (listReplace.Count > 0) || (listDelete.Count > 0);
        }

        public bool isFileSetForReplacement(string filename)
        {
            return listReplace.ContainsKey(FileNameHash.Compute(filename));
        }

        public bool isFileSetForDelete(string filename)
        {
            return listDelete.ContainsKey(FileNameHash.Compute(filename));
        }

#if (WITH_GUI)
        public void Execute(string outputFile, BackgroundWorker worker = null)
        {
            int count = 0;
            //int highPerc = 0;
#else
        public void Execute(string outputFile)
        {
#endif
            var inputPath = dlcBase.fileName;
            if (!File.Exists(inputPath))
                throw new FileNotFoundException("Error: the input file doesn't exists");

            /*string filePathToReplace = selectedFile;
            string fileToReplace = Path.GetFileName(filePathToReplace);
            FileNameHash fileToReplaceHash = new FileNameHash();*/

            string outputFileName = Path.GetFileNameWithoutExtension(inputPath);

            int inPointerBlockSize = 0;
            int outPointerEntry = 0x20;
            int outPointerEntryFileList = 0;
            int outPointerBlockSize = 0;
            int outPointerData = 0;

            int blocksToRemove = 0;
            int blocksToAdd = 0;

            using (FileStream input = File.OpenRead(inputPath),
                   output = File.OpenWrite(outputFile))
            {
                string dlcFileList = "";
                int outNumOfEntries = 0;
                //recreating the file list
                foreach (var kvp in listComplete)
                {
                    if (kvp.Value != action.delete)
                    {
                        outNumOfEntries++;
                        if (kvp.Key == DLCBase.fileListHash)
                            continue;
                        switch (kvp.Value)
                        {
                            case action.copy:
                            case action.replace:
                                dlcFileList += dlcBase.fileList[kvp.Key].fileName + Environment.NewLine;
                                break;
                            case action.add:
                                dlcFileList += listAdd[kvp.Key].fileName + Environment.NewLine;
                                break;
                        }
                    }
                }
                blocksToRemove += dlcBase.fileList[DLCBase.fileListHash].blockSizeArray.Length;
                blocksToAdd += (int)Math.Ceiling((double)dlcFileList.Length / (double)DLCBase.MaximumBlockSize);

                foreach (var kvp in listAdd)
                {
                    string fPath = kvp.Value.filePath;
                    if ((Path.GetExtension(fPath) != ".bik" &&
                         Path.GetExtension(fPath) != ".afc"))
                        blocksToAdd += (int)Math.Ceiling((double)DLCPack.Getsize(kvp.Value.filePath) / (double)DLCBase.MaximumBlockSize);
                }

                foreach (var kvp in listReplace)
                {
                    if (dlcBase.fileList[kvp.Key].blockSizeIndex != -1)
                    {
                        blocksToRemove += dlcBase.fileList[kvp.Key].blockSizeArray.Length;
                        blocksToAdd += (int)Math.Ceiling((double)DLCPack.Getsize(kvp.Value) / (double)DLCBase.MaximumBlockSize);
                    }
                }

                foreach (var kvp in listDelete)
                {
                    if (dlcBase.fileList[kvp.Key].blockSizeIndex != -1)
                        blocksToRemove += dlcBase.fileList[kvp.Key].blockSizeArray.Length;
                }

                var inputBlock = new byte[DLCBase.MaximumBlockSize];
                var outputBlock = new byte[DLCBase.MaximumBlockSize];
                //writing header of new sfar file
                input.Seek(0, 0);
                input.Read(inputBlock, 0, 32);
                output.Write(inputBlock, 0, 32);

                //getting initial blocks and data offsets
                inPointerBlockSize = 0x20 + (dlcBase.fileList.Count * 0x1E);
                outPointerBlockSize = 0x20 + ((int)outNumOfEntries * 0x1E);
                input.Seek(8, 0);
                int inDataOffset = input.ReadValueS32();
                int outBlockCount = ((inDataOffset - inPointerBlockSize) / 2) - blocksToRemove + blocksToAdd;
                outPointerData = outPointerBlockSize + (outBlockCount * 2);

                //writing new header's values
                output.Seek(8, 0);
                output.WriteValueS32(outPointerData);

                output.Seek(16, 0);
                output.WriteValueS32(outNumOfEntries);

                output.Seek(20, 0);
                output.WriteValueS32(outPointerBlockSize);

                if (verbose)
                {
                    Console.WriteLine("num entries: {0}", outNumOfEntries);
                    Console.WriteLine("data offset: {0:X8}", inDataOffset);
                    Console.WriteLine("blocks to remove: {0}", blocksToRemove);
                    Console.WriteLine("blocks to add: {0}", blocksToAdd);
                    Console.WriteLine("old block offset: {0:X8}", inPointerBlockSize);
                    Console.WriteLine("new block offset: {0:X8}", outPointerBlockSize);
                    Console.WriteLine("old block count: {0}", (inDataOffset - inPointerBlockSize) / 2);
                    Console.WriteLine("new block count: {0}", outBlockCount);
                    Console.WriteLine("pointer data: {0:X8}\n", outPointerData);
                }

                int numBlocks;
                int outDataOffset;
                int blockIndexCounter = 0;
                int outBlockIndex = 0;
                int fileSize = 0;

                int outInitialDataOffset = outPointerData;
                int outInitialBlockOffset = outPointerBlockSize;

                foreach (var kvp in listComplete)
                {
                    count++;

                    if (kvp.Value == action.delete)
                        continue;

                    if (kvp.Key == DLCBase.fileListHash)
                    {
                        //Console.WriteLine("File List Found at {0:X8}", outPointerEntry);
                        outPointerEntryFileList = outPointerEntry;
                        outPointerEntry += 0x1E;
                        continue;
                    }

                    sfarFile entry;
                    FileNameHash hashEntry = kvp.Key;

                    if (kvp.Value == action.add)
                    {
                        string fPath = listAdd[kvp.Key].filePath;
                        entry = new sfarFile();
                        entry.nameHash = kvp.Key;
                        entry.dataOffset = new long[1];
                        fileSize = (int)DLCPack.Getsize(fPath);
                        if ((Path.GetExtension(fPath) == ".bik" ||
                             Path.GetExtension(fPath) == ".afc"))
                            entry.blockSizeIndex = -1;
                    }
                    else
                    {
                        entry = dlcBase.fileList[kvp.Key];
                        fileSize = (int)entry.uncompressedSize;
                    }

                    outDataOffset = outPointerData;
                    outBlockIndex = blockIndexCounter;

                    /*#if (WITH_GUI)
                                        int perc = (int)Math.Ceiling((float)count++ / (float)listComplete.Count * 100);
                                        if (perc > highPerc)
                                        {
                                            highPerc = perc;
                                            if (perc > 100)
                                                perc = 100;
                                            worker.ReportProgress(perc);
                                        }
                    #endif*/

                    switch (kvp.Value)
                    {
                        case action.copy:
                            worker.ReportProgress(Convert.ToInt32(Math.Ceiling(((double)count/(double)listComplete.Count))*100));
                            if (entry.blockSizeIndex == -1)
                            {
                                inDataOffset = (int)entry.dataOffset[0];
                                input.Seek((long)inDataOffset, 0);
                                inputBlock = new byte[fileSize];
                                input.Read(inputBlock, 0, fileSize);

                                output.Seek((long)outPointerData, 0);
                                output.Write(inputBlock, 0, fileSize);
                                outPointerData += fileSize;

                                outBlockIndex = entry.blockSizeIndex;
                            }
                            else
                            {
                                numBlocks = (int)Math.Ceiling((double)fileSize / (double)DLCBase.MaximumBlockSize);
                                inDataOffset = (int)entry.dataOffset[0];
                                for (int i = 0; i < numBlocks; i++)
                                {
                                    uint blockSize = entry.blockSizeArray[i];
                                    if ((ushort)blockSize != entry.blockSizeArray[i])
                                        throw new Exception("different blocksizes");
                                    blockSize = blockSize == 0 ? DLCBase.MaximumBlockSize : blockSize;

                                    inputBlock = new byte[blockSize];
                                    input.Seek((long)inDataOffset, 0);
                                    input.Read(inputBlock, 0, (int)blockSize);
                                    inDataOffset += (int)blockSize;

                                    output.Seek((long)outPointerBlockSize, 0);
                                    if (blockSize == DLCBase.MaximumBlockSize)
                                        output.WriteValueU16(0);
                                    else
                                        output.WriteValueU16((ushort)blockSize);
                                    if (outPointerBlockSize > outInitialDataOffset)
                                        throw new Exception("Block index offset values out of range,\n  last block: " + blockIndexCounter + "\n  Pointer Block: " + outPointerBlockSize.ToString("X8") + "\n  Data Offset: " + outInitialDataOffset.ToString("X8"));
                                    outPointerBlockSize += 2;

                                    output.Seek((long)outPointerData, 0);
                                    output.Write(inputBlock, 0, (int)blockSize);
                                    if (output.Position - outPointerData != blockSize)
                                    {
                                        Console.WriteLine("  diff position: {0}, blocksize: {1}", output.Position - outPointerData, blockSize);
                                        throw new Exception("error writing file");
                                    }
                                    outPointerData += (int)blockSize;
                                }
                                blockIndexCounter += numBlocks;
                            }
                            break;
                        case action.add:
                        case action.replace:
                            string selectedFile;
                            if (kvp.Value == action.replace)
                            {
                                selectedFile = listReplace[kvp.Key];
                               // worker.ReportProgress(Convert.ToInt32(Math.Ceiling(((double)count / (double)listComplete.Count)) * 100));
                            }
                            else
                            {
                                selectedFile = listAdd[kvp.Key].filePath;
                                //worker.ReportProgress(Convert.ToInt32(Math.Ceiling(((double)count / (double)listComplete.Count)) * 100));
                            }
                            output.Seek((long)outPointerBlockSize, 0);
                            //compressing the replacing file
                            MemoryStream encStream = new MemoryStream();
                            ushort[] blockSizeArray;
                            FileStream streamFile = File.OpenRead(selectedFile);

                            if ((Path.GetExtension(selectedFile) == ".bik" ||
                                 Path.GetExtension(selectedFile) == ".afc") &&
                                entry.blockSizeIndex == -1)
                            {
                                streamFile.CopyTo(encStream);
                                outBlockIndex = -1;
                            }
                            else
                            {
                                DLCPack.CompressFile(streamFile, out blockSizeArray, encStream, worker);
                                for (int i = 0; i < blockSizeArray.Length; i++)
                                {
                                    output.WriteValueU16(blockSizeArray[i]);
                                }
                                outPointerBlockSize += (blockSizeArray.Length * 2);
                                blockIndexCounter += blockSizeArray.Length;
                            }
                            output.Seek((long)outPointerData, 0);
                            encStream.WriteTo(output);
                            outPointerData += (int)encStream.Length;

                            fileSize = (int)streamFile.Length;
                            streamFile.Close();
                            break;
                    }// end switch

                    

                    /*if((kvp.Value.dataOffset != outDataOffset) && verbose)
                    {
                        Console.WriteLine("  File to replace, in  data offset: {0:X8}",kvp.Value.dataOffset);
                        Console.WriteLine("  File to replace, out data offset: {0:X8}",outDataOffset);
                    }*/

                    /*if (verbose)
                    {
                        Console.WriteLine("HASH: {0}", entry.nameHash);
                        Console.WriteLine("    pointer Entry: {0:X8}", outPointerEntry);
                        Console.WriteLine("    pointer Block: {0:X8}", outPointerBlockSize);
                        Console.WriteLine("    in  Block Index Counter: {0}", entry.blockSizeIndex);
                        Console.WriteLine("    out Block Index Counter: {0}", blockIndexCounter);
                        Console.WriteLine("      in  data offset: {0:X8}", entry.dataOffset[0]);
                        Console.WriteLine("      out data offset: {0:X8}", outDataOffset);
                    }*/

                    output.Seek((long)outPointerEntry, 0);
                    output.WriteValueU32(hashEntry.A.Swap());
                    output.WriteValueU32(hashEntry.B.Swap());
                    output.WriteValueU32(hashEntry.C.Swap());
                    output.WriteValueU32(hashEntry.D.Swap());
                    output.WriteValueS32(outBlockIndex);
                    output.WriteValueS32(fileSize);
                    output.WriteValueU8(0);
                    output.WriteValueS32(outDataOffset);
                    output.WriteValueU8(0);
                    outPointerEntry += 0x1E;
                    if (outPointerEntry > outInitialBlockOffset)
                    {
                        throw new Exception("Entry index offset values out of range");
                    }
                    worker.ReportProgress(Convert.ToInt32(Math.Ceiling(((double)count / (double)listComplete.Count)) * 100));
                }// end of foreach

                //writing the file list entry, blocksizes & data
                outDataOffset = outPointerData;
                outBlockIndex = blockIndexCounter;
                {
                    MemoryStream streamRead = new MemoryStream(ASCIIEncoding.Default.GetBytes(dlcFileList));
                    ushort[] blockSizeArray;
                    MemoryStream encStream = new MemoryStream();
                    DLCPack.CompressFile(streamRead, out blockSizeArray, encStream);

                    output.Seek((long)outPointerBlockSize, 0);
                    for (int i = 0; i < blockSizeArray.Length; i++)
                    {
                        output.WriteValueU16(blockSizeArray[i]);
                    }
                    outPointerBlockSize += (blockSizeArray.Length * 2);
                    blockIndexCounter += blockSizeArray.Length;

                    output.Seek((long)outPointerData, 0);
                    encStream.WriteTo(output);
                    outPointerData += (int)encStream.Length;

                    fileSize = (int)streamRead.Length;

                    output.Seek((long)outPointerEntryFileList, 0);
                    output.WriteValueU32(DLCBase.fileListHash.A.Swap());
                    output.WriteValueU32(DLCBase.fileListHash.B.Swap());
                    output.WriteValueU32(DLCBase.fileListHash.C.Swap());
                    output.WriteValueU32(DLCBase.fileListHash.D.Swap());
                    output.WriteValueS32(outBlockIndex);
                    output.WriteValueS32((int)fileSize);
                    output.WriteValueU8(0x00);
                    output.WriteValueS32(outDataOffset);
                    output.WriteValueU8(0x00);
                    outPointerEntry = (int)output.Position;
                }

            }// end of using...

        }
    }
}
