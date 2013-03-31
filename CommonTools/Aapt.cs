﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace CommonTools
{
    public class Aapt
    {
        private static string mPathToAapt = Path.Combine(Environment.CurrentDirectory, "tools", "apktool", "aapt.exe");
        private static string mPathToApktool = Path.Combine(Environment.CurrentDirectory, "tools", "apktool", "apktool.bat");
        private static string mPathToSigner = Path.Combine(Environment.CurrentDirectory, "tools", "SignApk.jar");
        private static string mPathToZipAlign = Path.Combine(Environment.CurrentDirectory, "tools", "zipalign.exe");
        
        /// <summary>
        /// return 
        /// { 
        /// version name,
        /// version code,
        /// icon
        /// label
        /// }
        /// </summary>
        /// <param name="pathToApk"></param>
        /// <returns></returns>
        public static string getAppInfo(string pathToApk)
        {
            List<string> cmd = new List<string>();
            cmd.Add(mPathToAapt);
            cmd.Add("d");
            cmd.Add("badging");
            cmd.Add( pathToApk );

            StringBuilder data = new StringBuilder();

            Sys.Run(cmd.ToCommand(), (S,E) =>
            {
                data.Append(E.Data);
               
            });
            return data.ToString();
        }

        public static void DecodeApk(string pathToApkFile, string pathToDecodeFolder)
        {
            if (!File.Exists(pathToApkFile))
            {
                throw new Exception("Target apk is missing..");
            }

            List<String> cmd = new List<string>();
            cmd.Add(mPathToApktool);
            cmd.Add("d");
            cmd.Add("--no-src");
            cmd.Add("-f");
            cmd.Add(string.Format("\"{0}\"", pathToApkFile));
            cmd.Add(string.Format("\"{0}\"", pathToDecodeFolder));

            Sys.Run(cmd.ToCommand());
        }

        public static void BuildApk(string pathToDecodeFolder, string pathToDstApk)
        {
            if (File.Exists(pathToDstApk))
            {
                File.Delete(pathToDstApk);
            }

            List<String> cmd = new List<string>();
            cmd.Add(mPathToApktool);
            cmd.Add("b");
            cmd.Add(string.Format("\"{0}\"", pathToDecodeFolder));
            cmd.Add(string.Format("\"{0}\"", pathToDstApk));

            Sys.Run(cmd.ToCommand());
        }


        /// <summary>
        /// cmd: ApkSigner keystore storepw alias password input output
        /// </summary>
        /// <param name="channel"></param>
        public static void SignAPK(string keystore, string storepw, string entry, string keypw, string source, string dst)
        {
            string unSignedApk = source;
            string unzipAlignedApk = dst;

            if (!File.Exists(unSignedApk))
            {
                throw new Exception(string.Format("Can't find unsigned apk at {0}", unSignedApk));
            }

            if (File.Exists(unzipAlignedApk))
            {
                File.Delete(unzipAlignedApk);
            }

            List<string> cmd = new List<string>();

            cmd.Add( mPathToSigner );

            cmd.Add(keystore);
            cmd.Add(storepw);
            cmd.Add(entry);
            cmd.Add(keypw);

            cmd.Add(unSignedApk);
            cmd.Add(unzipAlignedApk);

            Sys.Run(cmd.ToCommand());
        }

        public static void ZipAlign(string source ,string dst)
        {
            string unzipAlignedApk = source;
            string finalApk = dst;

            if (!File.Exists(unzipAlignedApk))
            {
                throw new Exception(string.Format("Can't find unzipAligned apk at {0}", unzipAlignedApk));
            }
            if (File.Exists(finalApk))
            {
                File.Delete(finalApk);
            }
            List<string> cmd = new List<string>();

            cmd.Add( mPathToZipAlign );
            cmd.Add("-v");
            cmd.Add("4");
            cmd.Add(unzipAlignedApk); //input apk
            cmd.Add(finalApk); //output apk

            Sys.Run(cmd.ToCommand());
        }
    }
}
