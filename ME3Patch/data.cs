using System.Collections;

namespace ME3Patch
{
    public class hashdata//保存文件Hash，增量更新后有数据需修改。
    {
        public static Hashtable oldFilehashlist()//保存原始文件的MD5
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("0", "c69463b70321392337ed945a864e9dc5");//SFXGUI_Fonts.pcc
            ht.Add("1", "9e6f8f1c5183e0928235dd52c32283f9");//BIOGame_INT.tlk
            ht.Add("2", "4e4ccc80447a0b1144d3f5369ef7c4a9");//MassEffect3Config.exe（V1.5）
            ht.Add("3", "bb90b24309d8ea88d6e58d66cbe83b7a");//MassEffect3Config.xml
            ht.Add("4", "64ab5bae7ae4ad75108009d76c73389b");//DLC_HEN_PR
            ht.Add("5", "a80cc9089d01ba62fa465e70253a8ab4");//DLC_CON_MP1
            ht.Add("6", "949a4197ac8fb97221f63da41f61c6b7");//DLC_CON_MP2
            ht.Add("7", "a0f9f2acdba80acba100218f205e385e");//DLC_CON_END
            ht.Add("8", "69fd670cac701dc16d034fb5ebb17524");//DLC_CON_MP3
            ht.Add("9", "d05977324e5ef172e8d0f10ec664ab9f");//gun01
            ht.Add("10", "3b9b37d842378e96038c17389dd63032");//exp01
            ht.Add("11", "f025e9b197bfa9e0ce24ca7aefc7b00f");//UPD01(V1.5,V1.4 70b82aabf1a3e78bae567e9e9ceef57d)
            ht.Add("12", "10987f6f49a786637b045ba38e1cb78f");//MP4
            ht.Add("13", "6d7fa053fac1696c6b64ea20669db5c0");//GUN02
            ht.Add("14", "d27098a14da986f4562bda557ed778cc");//APP01
            ht.Add("15", "ba6f1055dff2cc63c72c34b59a2df9cb");//EXP02
            ht.Add("16", "77c5584cff4726ad754cbecefa38adad");//UPD02
            ht.Add("17", "4645cc530f4f309dc7be4eb1dffccab6");//MP5
            ht.Add("18", "f4c66724f2cf26e4bbe3b62d9024b709");//EXP03-1
            ht.Add("19", "b361c4bca1ac106dbb0c4b629e7c3022");//EXP03-2
            ht.Add("20", "ea34559050385d928e45db218caa4007");//Dh1
            return ht;
        }
        public static Hashtable filename()//保存文件的名
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("0", "字库");//SFXGUI_Fonts.pcc
            ht.Add("1", "游戏本体");//BIOGame_INT.tlk
            ht.Add("2", "配置程序");//MassEffect3Config.exe
            ht.Add("3", "配置程序资源文件");//MassEffect3Config.xml
            ht.Add("4", "From Ashes（涅槃，单人）");//DLC_HEN_PR
            ht.Add("5", "Resurgence（复苏，多人）");//DLC_CON_MP1
            ht.Add("6", "Rebellion（反抗，多人）");//DLC_CON_MP2
            ht.Add("7", "Extended Cut（加长版结局，单人）");//DLC_CON_END
            ht.Add("8", "Earth（地球，多人）");//DLC_CON_MP3
            ht.Add("9", "Firefight Pack（火力包，单人）");//DLC_CON_MP3
            ht.Add("10", "Leviathan（利维坦，单人）");//DLC_CON_MP3
            ht.Add("11", "V1.0.4升级补丁");//DLC_CON_MP3
            ht.Add("12", "Retaliation（报复，多人）");//DLC_CON_MP3
            ht.Add("13", "Groundside Pack（地面抵抗力量包，单人）");//DLC_CON_MP3
            ht.Add("14", "Appearance Pack 1（服装包1，单人）");//DLC_CON_MP3
            ht.Add("15", "Omega（欧米伽，单人）");//DLC_CON_EXP02
            ht.Add("16", "V1.0.5升级补丁");//DLC_CON_EXP02
            ht.Add("17", "Reckoning（清算，多人）");//DLC_CON_EXP02
            ht.Add("18", "Citidel-1（神堡-1，单人）");//DLC_CON_EXP02
            ht.Add("19", "Citidel-2（神堡-2，单人）");//DLC_CON_EXP02
            ht.Add("20", "Genesis 2（起源2，单人）");//DLC_DH2
            return ht;
        }
        public static Hashtable newFilehashlist()//保存单语汉化文件的Md5
        {
            Hashtable ht = new Hashtable();
            ht.Add("0", "45909ca2b054b1a4b51803e206f0b6e3");//字库
            ht.Add("1", "8a30cb352e6b29703c459eb9dac38ec9");//本体
            ht.Add("2", "0d575368448f5836cb759caca1ffa611");//MassEffect3Config.exe
            ht.Add("3", "239b9f5bc2f31ebbccaadfb9ef479f0a");//MassEffect3Config.xml
            ht.Add("4", "05e3455ee79c9dae20c0742a2ea4770d");//Pr  Default.sfar
            ht.Add("5", "03c27e78af42240fa5b9a57c64676ef0");//mp1  Default.sfar
            ht.Add("6", "475177a684c3aaeb184c72aeae74e45f");//mp2
            ht.Add("7", "3150823a514f9190af67991dcb9d1cd7");//EC
            ht.Add("8", "3c773c7ff99a0b99eb8ccb09b7d08642");//DLC_CON_MP3
            ht.Add("9", "d08c64315dca87b75c0b7208dbf4b0ed");//GUN01
            ht.Add("10", "5ea6fe0fe41e0b6e9f8784ebeb344396");//EXP001
            ht.Add("11", "0d4fdc2bb672bd11aef2297ff25dff26");//Patch
            ht.Add("12", "2e67975493408eedcf0146e317b15a24");//MP4
            ht.Add("13", "7cc866c9f6500f3656b310b712558f57");//gun2
            ht.Add("14", "a9e62bfddaaaac4c92ef99cf2461fb38");//app
            ht.Add("15", "7d664f11e63e4af5981297158c48108c");//omg
            ht.Add("16", "24164b51eced53e46e34fd1c783c2905");//omg
            ht.Add("17", "6b4db5ab1e9783ee346ec92ebd36c041");//omg
            ht.Add("18", "640b5af9587a42bc013f19ecbf8e95c5");//omg
            ht.Add("19", "39880d25dd68f2560b13af2ed500a82b");//omg
            ht.Add("20", "2e61490083ef7f7bd7293ab32533ce90");//dh1
            return ht;
        }
        public static Hashtable new2Filehashlist()//保存双语汉化文件的Md5
        {
            Hashtable ht = new Hashtable();
            ht.Add("0", "45909ca2b054b1a4b51803e206f0b6e3");//字库
            ht.Add("1", "b5b9ef73d7cce95ca8292c40d040d2fd");//本体
            ht.Add("2", "0d575368448f5836cb759caca1ffa611");//MassEffect3Config.exe
            ht.Add("3", "239b9f5bc2f31ebbccaadfb9ef479f0a");//MassEffect3Config.xml
            ht.Add("4", "a3293e32aabae8383c6748c7a054ee14");//Pr
            ht.Add("5", "03c27e78af42240fa5b9a57c64676ef0");//mp1
            ht.Add("6", "475177a684c3aaeb184c72aeae74e45f");//mp2
            ht.Add("7", "535c3adb5810e3bcf9ba3766d5acc5f3");//EC
            ht.Add("8", "3c773c7ff99a0b99eb8ccb09b7d08642");//DLC_CON_MP3
            ht.Add("9", "d08c64315dca87b75c0b7208dbf4b0ed");//GUN01
            ht.Add("10", "aac1fd8914cfffb85265dc1f9d9a696e");//EXP001
            ht.Add("11", "f1a070d03b40ba2dfab14294a75cc2e1");//Patch
            ht.Add("12", "4feb139b1abd08d5f713e39fdf2a9e6f");//MP4
            ht.Add("13", "cc55c1b8c10cbbbcb9eea23393a9651c");//gun02
            ht.Add("14", "a9e62bfddaaaac4c92ef99cf2461fb38");//app
            ht.Add("15", "493c1ec5575ba6433919f914bc9926f9");//omg
            ht.Add("16", "5be7642edcfa5c934370eda190bef33d");//omg
            ht.Add("17", "474615c10985adca372ac1304a591134");//omg
            ht.Add("18", "faabfc5a746d1a4b4bb51871467b7ca9");//omg
            ht.Add("19", "4e462727829841e8790ae92b4fec6116");//omg
            ht.Add("20", "b9775b87f73921e140dc086746a11624");//omg

            return ht;
        }
        public static Hashtable replaceFilehashlist()//保存还原后DLC文件的Md5
        {
            Hashtable ht = new Hashtable();

            ht.Add("0", "b1d2aafad89b3ac27646efc8a436a3f8");//i+1=汉化还原后的文件md5
            ht.Add("1", "7d2e2455e1f450d71edc5ad383e0c63f");//Mp1
            ht.Add("2", "7b1ac759f4014178cc9d72bfa0b33c9d");//Mp2
            ht.Add("3", "2ee1af066436ac62949d641d4f72120a");//EC
            ht.Add("4", "c72a90b4669e7af88c997986b851c3a0");//DLC_CON_MP3
            ht.Add("5", "44b2a02eb7969bbe5136594644f86f7c");//gun01
            ht.Add("6", "42e5759d4ed06d83b273041cdab814d3");//EXP01
            ht.Add("7", "2a7b72b840faa80a5c1bd02a4a6c4f3d");//UPD
            ht.Add("8", "0bb68d237872bf1029281b9d17cba5ac");//MP4
            ht.Add("9", "04af8a63d61e9e52f48bceb03ad949b1");//GUN02
            ht.Add("10", "d8999173190920e4723a6fa074042dd8");//APP01
            ht.Add("11", "eb71c7f8f7e88a344c223eaa9590112e");//EXP02
            ht.Add("12", "b48344142cc212c33037e461b0e4d0e4");//omg
            ht.Add("13", "7392624a4ac1d8662aa10a68ca9b54d2");//omg
            ht.Add("14", "b0c04d0d1e8e9f3125a6ab83b0b26776");//omg
            ht.Add("15", "8c8a660c4bfe81859906d33b19ff1c1d");//omg
            ht.Add("16", "cf88ff0359a319d76905741d3ea59f03");//omg


            return ht;
        }
        public static Hashtable cupdFilehashlist()//保存单语可升级文件的MD5
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("0", "5e34ba735db1bbad03bf5b01f52d46ab");//1
            ht.Add("1", "2dfff1adf2df53a91d7415f8b296688c");//2
            ht.Add("2", "bbf9596c7a8dd156e9669c158d0051ef");//3-0.8
            ht.Add("3", "1c39858678bcc82d3f9d9ccddf386c22");//4
            ht.Add("4", "7b619bb1a140b467549dd85dda584ed4");//5
            ht.Add("5", "8b989152be2be67b30712158d9021141");//6
            ht.Add("6", "bbf9596c7a8dd156e9669c158d0051ef");//7-1.0  BIOGame_INT.tlk
            ht.Add("7", "941239605e45634f5af28a25f446a274");//8-1.1  BIOGame_INT.tlk
            ht.Add("8", "15c9a296eb4dc44d9b73c8d0865bf8a7");//9-3.0  BIOGame_INT.tlk
            //ht.Add("9", "5ea6fe0fe41e0b6e9f8784ebeb344396");//10
            ht.Add("9", "91961a972eec05bfb07ddab2df06a1d5");//12-1.1  mp2
            ht.Add("10", "ba23c6affc7025a935c6d152348ece07");//13-2.0  BIOGame_INT.tlk
            ht.Add("11", "7d0c845be64526a3031cedfacd438a72");//14-2.0  pr
            //ht.Add("13", "265e8979696f003326585ccab8d9a2d8");//15-2.0  ec
            //ht.Add("14", "cf5f45b9f76d0a8d2026db58c5a415e7");//16-2.0  mp2
            ////2.2
            //ht.Add("15", "04dedb5417e00f769e73863bf163bfcc");//mp1
            //ht.Add("16", "7ece83d4e364167c91fec105aac101f8");//EC
            //ht.Add("17", "dac1dd93b02288a7f3d85aff778b089b");//DLC_CON_MP3
            //ht.Add("18", "f8cdc57393b1faaed497faaa410cbdda");//字库
            //ht.Add("19", "e97124f7db0a250a32046975b07a61fa");//本体
            ////2.4
            //ht.Add("20", "a8f5580fea7b0b0b00149a0ed988b36d");//MassEffect3Config.exe
            //ht.Add("21", "61fe031693368430c06905c51041998f");//字库
            //ht.Add("22", "0a2adeb97a9208e3cbbd0369a63de773");//EXP001
            //ht.Add("23", "516a836aa4e866bc2cdc8085b1c1eeef");//本体
            //ht.Add("0", "5e34ba735db1bbad03bf5b01f52d46ab");//字库
            return ht;
        }
        public static Hashtable cupd2Filehashlist()//保存双语可升级文件的MD5
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("0", "5e34ba735db1bbad03bf5b01f52d46ab");//1
            ht.Add("1", "941239605e45634f5af28a25f446a274");//2
            ht.Add("2", "e58212d69a770e17b8358f293e8470ee");//3-0.8
            ht.Add("3", "8c29c62c2e736a0d5086e14cdd841023");//4
            ht.Add("4", "ff337c0bf8c2f35bc97785ac502d28e4");//5
            ht.Add("5", "9721229a004efa9ad4a463ed05c0a29a");//6
            ht.Add("6", "1748d528412c835e3f0cd3e2d7a21342");//7-1.0  BIOGame_INT.tlk
            ht.Add("7", "fca9460e9d82a261621c8ce6e12a5fa8");//8-1.1  BIOGame_INT.tlk
            ht.Add("8", "799d664b7b80d495300c9def890706ad");//9-3.0  BIOGame_INT.tlk
            ht.Add("9", "c443dbfb8c071bc845f5e49e1fe0802d");//10
            ht.Add("10", "f43bca2e24fdc824f574172498ad3643");//12-1.1  mp2
            ht.Add("11", "f313815529edf009867d7d4b250fc72f");//13-2.0  BIOGame_INT.tlk
            ht.Add("12", "e5b94682501effcf5f46fbb3afaa6f92");//14-2.0  pr
            //ht.Add("13", "265e8979696f003326585ccab8d9a2d8");//15-2.0  ec
            //ht.Add("14", "cf5f45b9f76d0a8d2026db58c5a415e7");//16-2.0  mp2
            ////2.2
            //ht.Add("15", "04dedb5417e00f769e73863bf163bfcc");//mp1
            //ht.Add("16", "7ece83d4e364167c91fec105aac101f8");//EC
            //ht.Add("17", "dac1dd93b02288a7f3d85aff778b089b");//DLC_CON_MP3
            //ht.Add("18", "f8cdc57393b1faaed497faaa410cbdda");//字库
            //ht.Add("19", "e97124f7db0a250a32046975b07a61fa");//本体
            ////2.4
            //ht.Add("20", "a8f5580fea7b0b0b00149a0ed988b36d");//MassEffect3Config.exe
            //ht.Add("21", "61fe031693368430c06905c51041998f");//字库
            //ht.Add("22", "0a2adeb97a9208e3cbbd0369a63de773");//EXP001
            //ht.Add("23", "516a836aa4e866bc2cdc8085b1c1eeef");//本体
           // ht.Add("0", "5e34ba735db1bbad03bf5b01f52d46ab");//字库
            //ht.Add("7", "81836651ca8e6ade38ce216c4d16bbd3");//2
            return ht;
        }
        public static Hashtable fileversion()//保存旧版版文件的校验值
        {
            Hashtable ht = new Hashtable();
            //原双语版
            ht.Add("0", "d04e44b66a95a14bcda0c16aa23bfb5c");//2.0
            ht.Add("1", "70dee7685a092314af3c1de224fe8b6e");//2.1
            ht.Add("2", "fc368ddb4fdacc9b2640716b6452b7b9");//2.2
            ht.Add("3", "da1e372617188320d9e1fa13e01d2308"); //2.4
            ht.Add("4", "fe0a93fadbbef3dc709c70b1d2abbe81"); //2.6
            //原单语版
            ht.Add("5", "67236d34f96396b92dfd8d9fe3e4f548");//0.8
            ht.Add("6", "b6eea2a242105097efeb88bc0173e4d5");//1.0
            ht.Add("7", "db0193603ea72aedf8788148da045c37");//1.1
            ht.Add("8", "e97124f7db0a250a32046975b07a61fa");//2.0
            ht.Add("9", "07652a6bb94965b0158272cb8a379b96");//2.2
            ht.Add("10", "516a836aa4e866bc2cdc8085b1c1eeef");//2.4
            ht.Add("11", "32db4cbc178550551586bb19bdebc408");//2.6
            //2.7
            ht.Add("12", "027206bb55bb7e4fb123a47b04310823");
            ht.Add("13", "96a53ed270b83c3a5d169de89c7993bc");
            //V1.4OLD
            ht.Add("14", "70b82aabf1a3e78bae567e9e9ceef57d");
            //其他语言版本
            ht.Add("15", "8f6dc0c61f1a17d9993e7645d72c3ec5");//3.0
            ht.Add("16", "dcd1ec13b5923ad1568db5db020e44b9");//3.2
ht.Add("17", "176b5cdd1f24346d578e9233a02a2e71");//3.0
            ht.Add("18", "a7fd603cbe53de79a727967401e0cf2b");//3.2
            return ht;
        }
        public static Hashtable filePathhashlist()//保存文件的相对路径
        {
            Hashtable ht = new Hashtable();
            ht.Add("0", "\\BIOGame\\CookedPCConsole\\");
            ht.Add("1", "\\BIOGame\\CookedPCConsole\\");
            ht.Add("2", "\\Binaries\\");
            ht.Add("3", "\\Data\\");
            ht.Add("4", "\\BIOGame\\DLC\\DLC_HEN_PR\\CookedPCConsole\\");
            ht.Add("5", "\\BIOGame\\DLC\\DLC_CON_MP1\\CookedPCConsole\\");
            ht.Add("6", "\\BIOGame\\DLC\\DLC_CON_MP2\\CookedPCConsole\\");
            ht.Add("7", "\\BIOGame\\DLC\\DLC_CON_END\\CookedPCConsole\\");
            ht.Add("8", "\\BIOGame\\DLC\\DLC_CON_MP3\\CookedPCConsole\\");
            ht.Add("9", "\\BIOGame\\DLC\\DLC_CON_GUN01\\CookedPCConsole\\");
            ht.Add("10", "\\BIOGame\\DLC\\DLC_EXP_Pack001\\CookedPCConsole\\");
            ht.Add("11", "\\BIOGame\\DLC\\DLC_UPD_Patch01\\CookedPCConsole\\");
            ht.Add("12", "\\BIOGame\\DLC\\DLC_CON_MP4\\CookedPCConsole\\");
            ht.Add("13", "\\BIOGame\\DLC\\DLC_CON_GUN02\\CookedPCConsole\\");
            ht.Add("14", "\\BIOGame\\DLC\\DLC_CON_APP01\\CookedPCConsole\\");
            ht.Add("15", "\\BIOGame\\DLC\\DLC_EXP_Pack002\\CookedPCConsole\\");
            ht.Add("16", "\\BIOGame\\DLC\\DLC_UPD_Patch02\\CookedPCConsole\\");
            ht.Add("17", "\\BIOGame\\DLC\\DLC_CON_MP5\\CookedPCConsole\\");
            ht.Add("18", "\\BIOGame\\DLC\\DLC_EXP_Pack003_Base\\CookedPCConsole\\");
            ht.Add("19", "\\BIOGame\\DLC\\DLC_EXP_Pack003\\CookedPCConsole\\");
            ht.Add("20", "\\BIOGame\\DLC\\DLC_CON_DH1\\CookedPCConsole\\");
            return ht;
        }
        
        public static Hashtable filestatus()//初始化组件状态表
        {
            Hashtable ht = new Hashtable();
            return ht;
        }

    }
}
