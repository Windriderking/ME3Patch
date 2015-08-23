using System;
using System.Runtime.InteropServices;
using System.IO;
using Gibbed.IO;
using System.Collections;
using System.Windows.Forms;
using ME3Patch;
namespace VPatch4cs
{



    unsafe class DoPatch
    {
        public delegate void SetValue(ProgressBar C, int Value);
        public delegate void SetText(Label C, string Value);
        public static void DoSetValue(ProgressBar C, int Value)
        {
            C.Value = Value;
        }
        public static void DoSetMax(ProgressBar C, int Value)
        {
            C.Maximum = Value;
            C.Minimum = 0;
        }
        public static void DoSetText(Label C, string Value)
        {
            C.Text = Value;
        }
        [DllImport("kernel32")]
        public static extern void GetSystemTimeAsFileTime(FILETIME *lpSystemTimeAsFileTime);
        [DllImport("kernel32")]
        public static extern Int32 SetFileTime(HandleRef hFile, out FILETIME *lpCreationTime, out FILETIME *lpLastAccessTime, out FILETIME *lpLastWriteTime);
        [DllImport("kernel32")]
        public static extern bool GetFileTime(HandleRef hFile, out FILETIME *lpCreationTime, out FILETIME *lpLastAccessTime, out FILETIME *lpLastWriteTime);
        public static unsafe int Patch(string hPatchs, string hSources, string    hDests,ProgressBar pBar,Label lab,int FileID)
        {
            try
            {
                if (pBar.InvokeRequired == true)
                {
                    SetValue PSetValue = new SetValue(DoSetValue);
                    SetText LSetText = new SetText(DoSetText);
                    pBar.Invoke(PSetValue, new Object[] { pBar, 0 });
                    lab.Invoke(LSetText, new Object[] { lab, "0%" });
                }
                UInt32 BLOCKSIZE = 16384;
                UInt32 temp = 0;
                UInt32 read;
                byte[] source_md5 = new byte[16];
                byte[] patch_dest_md5 = new byte[16];
                byte[] block = new byte[BLOCKSIZE];
                int MD5Mode = 0;
                UInt32 patches = 0;
                int already_uptodate = 0,j=1;
                double blocks;
                FILETIME targetModifiedTime;
                DateTime dte = new DateTime() ;
                //write回调方法
                FileStream Patchs = new FileStream(hPatchs, FileMode.Open, FileAccess.Read, FileShare.Read);
                FileStream Sources = new FileStream(hSources, FileMode.Open, FileAccess.Read, FileShare.Read);
                FileStream Dests = new FileStream(hDests, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                Hashtable Data = Program.filehash;
                // special 'addition' for the dll: since the patch file is now
                // in a seperate file, the VPAT header might be right at the start
                // of the file, and a pointer at the end of the file is probably missing
                // (because all patch generator versions don't append it, the linker/gui
                //  does this).
                Patchs.Seek(0, SeekOrigin.Begin);
                temp = Patchs.ReadValueU32(); read = 4;
                // it's not at the start of file -> there must be a pointer at the end of
                // file then
                if (temp != 0x54415056)
                {
                    Patchs.Seek(-4, SeekOrigin.End);
                    temp = Patchs.ReadValueU32(); read = 4;
                    Patchs.Seek(temp, SeekOrigin.Begin);
                    temp = Patchs.ReadValueU32(); read = 4;
                    if (temp != 0x54415056)
                        return result.PATCH_CORRUPT;
                }

                // target file date is by default the current system time
                GetSystemTimeAsFileTime(&targetModifiedTime);

                // read the number of patches in the file
                patches = Patchs.ReadValueU32(); read = 4;
                if (Convert.ToBoolean(patches & 0x80000000)) MD5Mode = 1;
                // MSB is now reserved for future extensions, anyone wanting more than
                // 16 million patches in a single file is nuts anyway
                patches = patches & 0x00FFFFFF;

                if (!Convert.ToBoolean(MD5Mode))
                {
                    return result.PATCH_UNSUPPORTED;
                }
                else
                {
                    source_md5 = checksum.FileMD5(Sources, Data, FileID,Program.AdvMOD);
                    if (source_md5 == null)
                        return result.PATCH_ERROR;
                }
               
                //pBar.Maximum = Convert.ToInt32(patches);
                while (Convert.ToBoolean(patches--))
                {
                    int patch_blocks = 0, patch_size = 0;

                    // flag which needs to be set by one of the checksum checks
                    int currentPatchMatchesChecksum = 0;

                    // read the number of blocks this patch has
                    patch_blocks = Convert.ToInt32(Patchs.ReadValueU32()); blocks = patch_blocks; read = 4;
                    if (!Convert.ToBoolean(patch_blocks))
                    {
                        return result.PATCH_CORRUPT;
                    }
                    if (pBar.InvokeRequired == true)
                    {
                        SetValue PSetValue = new SetValue(DoSetMax);
                        pBar.Invoke(PSetValue, new Object[] { pBar, Convert.ToInt32(patch_blocks) });
                    }
                    // read checksums
                    if (!Convert.ToBoolean(MD5Mode))
                    {
                        return result.PATCH_UNSUPPORTED;
                    }
                    else
                    {
                        int md5index;
                        byte[] patch_source_md5 = new byte[16];
                        patch_source_md5 = Patchs.ReadBytes(16); read = 16;
                        if (patch_source_md5 == null)
                        {
                            return result.PATCH_CORRUPT;
                        }
                        patch_dest_md5 = Patchs.ReadBytes(16); read = 16;
                        if (patch_dest_md5 == null)
                        {
                            return result.PATCH_CORRUPT;
                        }
                        // check to see if it's already up-to-date for some patch
                        for (md5index = 0; md5index < 16; md5index++)
                        {
                            if (source_md5[md5index] != patch_dest_md5[md5index]) break;
                            if (md5index == 15) already_uptodate = 1;
                        }
                        for (md5index = 0; md5index < 16; md5index++)
                        {
                            if (source_md5[md5index] != patch_source_md5[md5index]) break;
                            if (md5index == 15) currentPatchMatchesChecksum = 1;
                        }
                    }
                    // read the size of the patch, we can use this to skip over it
                    patch_size = Convert.ToInt32(Patchs.ReadValueU32()); read = 4;
                    if (patch_size == null)
                    {
                        return result.PATCH_CORRUPT;
                    }

                    if (Convert.ToBoolean(currentPatchMatchesChecksum))
                    {
                        while (Convert.ToBoolean(patch_blocks--))
                        {
                            if (pBar.InvokeRequired == true)
                            {
                                SetValue PSetValue = new SetValue(DoSetValue);
                                SetText LSetText = new SetText(DoSetText);
                                pBar.Invoke(PSetValue, new Object[] { pBar, Convert.ToInt32(j++) });
                                var per = Convert.ToDouble(j-1)/Convert.ToDouble(blocks);
                                lab.Invoke(LSetText, new Object[] { lab, Convert.ToString(Math.Ceiling(per*100)) + "%" });
                                Application.DoEvents();
                            }
                            Byte blocktype = 0;
                            UInt32 blocksize = 0;
                            blocktype = Convert.ToByte(Patchs.ReadByte()); read = 1;
                            if (blocktype==null)
                            {
                                return result.PATCH_CORRUPT;
                            }
                            switch (blocktype)
                            {
                                case 1:
                                case 2:
                                case 3:
                                    if (blocktype == 1)
                                    {
                                        Byte x;
                                        x = Convert.ToByte(Patchs.ReadByte()); read = 1;
                                        blocksize = Convert.ToUInt32(Convert.ToBoolean(x) ? x : 0);
                                    }
                                    else if (blocktype == 2)
                                    {
                                        UInt16 x;
                                        x = Patchs.ReadValueU16(); read = 2;
                                        blocksize = Convert.ToUInt32(Convert.ToBoolean(x) ? x : 0);
                                    }
                                    else
                                    {
                                        UInt32 x;
                                        x = Patchs.ReadValueU32(); read = 4;
                                        blocksize = Convert.ToUInt32(Convert.ToBoolean(x) ? x : 0);
                                    }
                                    temp = Patchs.ReadValueU32(); read = 4;
                                    if (!Convert.ToBoolean(blocksize) || temp == null || read != 4)
                                        return result.PATCH_CORRUPT;
                                    Sources.Seek(temp, SeekOrigin.Begin);
                                    //SetFilePointer(hSource, temp, 0, EMoveMethod.Begin);

                                    do
                                    {
                                        Sources.Read(block, 0, Convert.ToInt32(Math.Min(BLOCKSIZE, blocksize))); read = Math.Min(BLOCKSIZE, blocksize);
                                        if (block == null)
                                        {
                                            return result.PATCH_ERROR;
                                        }
                                        //IAsyncResult writeResult = Dests.BeginWrite(block,0,Convert.ToInt32(read),writeCallBack,"Write Target File");
                                        //Dests.EndWrite(writeResult);
                                        Dests.Write(block, 0, Convert.ToInt32(read)); 
                                        temp = read;
                                        //WriteFile(hDest, block, read, &temp, NULL);
                                        if (temp != Math.Min(BLOCKSIZE, blocksize))
                                            return result.PATCH_ERROR;
                                        blocksize -= temp;
                                    } while (Convert.ToBoolean(temp));

                                    break;

                                case 5:
                                case 6:
                                case 7:
                                    if (blocktype == 5)
                                    {
                                        Byte x;
                                        x = Convert.ToByte(Patchs.ReadByte()); read = 1;
                                        blocksize = Convert.ToUInt32(Convert.ToBoolean(x) ? x : 0);
                                    }
                                    else if (blocktype == 6)
                                    {
                                        UInt16 x;
                                        x = Patchs.ReadValueU16(); read = 2;
                                        blocksize = Convert.ToUInt32(Convert.ToBoolean(x) ? x : 0);
                                    }
                                    else
                                    {
                                        UInt32 x;
                                        x = Patchs.ReadValueU32(); read = 4;
                                        blocksize = Convert.ToUInt32(Convert.ToBoolean(x) ? x : 0);
                                    }

                                    if (!Convert.ToBoolean(blocksize))
                                        return result.PATCH_CORRUPT;

                                    do
                                    {
                                        Patchs.Read(block, 0, Convert.ToInt32(Math.Min(BLOCKSIZE, blocksize))); read = Math.Min(BLOCKSIZE, blocksize);
                                        if (block == null)
                                        {
                                            return result.PATCH_CORRUPT;
                                        }
                                        //IAsyncResult writeResult = Dests.BeginWrite(block, 0, Convert.ToInt32(read), writeCallBack, "Write Target File"); 
                                        //Dests.EndWrite(writeResult);
                                        Dests.Write(block, 0, Convert.ToInt32(read));
                                        temp = read;
                                        //WriteFile(hDest, block, read, &temp, NULL);
                                        if (temp != Math.Min(BLOCKSIZE, blocksize))
                                            return result.PATCH_ERROR;
                                        blocksize -= temp;
                                    } while (Convert.ToBoolean(temp));

                                    break;

                                case 255:   // read the file modified time from the patch
                                    targetModifiedTime.dwLowDateTime = (int)Patchs.ReadValueU32();
                                    targetModifiedTime.dwHighDateTime = (int)Patchs.ReadValueU32();
                                    
                                    /////////////////////////////////////////////////////////////////            //from System.Runtime.InteropServices.FILETIME to System.DateTime            /////////////////////////////////////////////////////////////////            
                                    long _Value = (long) targetModifiedTime.dwHighDateTime << 32 | (long)(uint)targetModifiedTime.dwLowDateTime;
                                    dte = DateTime.FromFileTimeUtc(_Value);
                                    read = Convert.ToUInt32(Marshal.SizeOf(targetModifiedTime));
                                    //if(targetModifiedTime) {
                                    //return result.PATCH_CORRUPT;
                                    //}
                                    break;

                                default:
                                    return result.PATCH_CORRUPT;
                            }
                        }
                        if (!Convert.ToBoolean(MD5Mode))
                        {
                            return result.PATCH_UNSUPPORTED;
                        }
                        else
                        {
                            
                           //int md5index;
                            byte[] dest_md5 = new byte[16];
                            Dests.Close(); 
                            Patchs.Close(); 
                            Sources.Close();
                            File.Delete(hPatchs);
                            File.Delete(hSources);
                            File.Move(hDests, hSources);
                            File.SetLastWriteTime(hSources, dte);
                            //FileStream Dest = new FileStream(hSources, FileMode.Open, FileAccess.Read, FileShare.None);
                            //dest_md5 = checksum.FileMD5(Dest, Data, -1);
                            //Dest.Close();
                            //if (dest_md5 == null)
                            //{
                             //   return result.PATCH_ERROR;
                           // }
                            //for (md5index = 0; md5index < 16; md5index++)
                            //{
                             //   if (dest_md5[md5index] != patch_dest_md5[md5index]) return result.PATCH_ERROR;
                            //}
                        }
                        // set file time
                       
                        //SetFileTime(hDest, NULL, NULL, &targetModifiedTime);                      
                        return result.PATCH_SUCCESS;
                    }
                    else
                    {
                        Patchs.Seek(patch_size, SeekOrigin.Current);
                        //SetFilePointer(hPatch, patch_size, NULL, EMoveMethod.Current);
                    }
                }

                // if already up to date, it doesn't matter that we didn't match
                if (Convert.ToBoolean(already_uptodate))
                {
                    return result.PATCH_UPTODATE;
                }
                else
                {
                    return result.PATCH_NOMATCH;
                }
            }
            catch (Exception Err)
            {
                MessageBox.Show("出现错误：" + Err.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return result.PATCH_ERROR;
            }
        }
        
}

    }


