using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DuplicateFileDetect
{
    public class Pair
    {
        public Pair(string fileOne, string fileTwo)
        {
            this.FileOne = fileOne;
            this.FileTwo = fileTwo;
        }
        public string FileOne { get; set; }
        public string FileTwo { get; set; }
    }

    class Program
    {
        static long HashSampleSizeBytes = 5000;

        static void Main(string[] args)
        {

            if (args.Length > 0)
            {
                string folderPath = args[0];

                if (Directory.Exists(folderPath))
                {
                    if (Directory.GetFiles(folderPath).Length > 2)
                    {
                        Console.WriteLine("Files With Duplcate Contents");
                        foreach (Pair matchFiles in ListDuplicateFiles(folderPath))
                        {
                            Console.WriteLine("{0} == {1}",matchFiles.FileOne, matchFiles.FileTwo);
                        }
                    }
                    else
                    {
                        Console.Write("Insufficient files");
                    }
                }
                else
                {
                    Console.WriteLine("folder does not exist.");
                }

            }
            else
            {
                Console.WriteLine("Please provide path to search for duplicates.");
            }

            Console.WriteLine("Press Any Key to Exit");
            Console.ReadKey();
        }

        private static List<Pair> ListDuplicateFiles(string folderPath)
        {
            //hash, filename
            Dictionary<string,string> allFileNameDic = new Dictionary<string,string>();
            List<Pair> potentialDupList = new List<Pair>();
            List<Pair> confirmedDupList = new List <Pair>();


            //1st pass hash 1st 5000 bytes todo: 5k arbitrary, tests to determine optimal value
            foreach (string fileName in Directory.EnumerateFiles(folderPath))
            {
                string thisHash = GetFileHash(fileName, HashSampleSizeBytes);

                if(!allFileNameDic.Keys.Contains(thisHash))
                {
                    allFileNameDic.Add(thisHash,fileName);
                }
                else
                {
                    //Potential Dupe - add filename of potential dup and this filename to potential list
                    potentialDupList.Add(new Pair(fileName, allFileNameDic[thisHash]));
                }
                
            }

            //2nd pass - Hash full files & compare; add to output list if full hash match
            
            foreach(Pair potentialPair in potentialDupList)
            {
                if(GetFileHash(potentialPair.FileOne) == GetFileHash(potentialPair.FileTwo))
                {
                    //full match
                    confirmedDupList.Add(potentialPair);
                }
            }


            //remove last comma, split to string array
            return confirmedDupList;
        }
        /// <summary>
        /// Returns MD5 Hash of full or partial file
        /// </summary>
        /// <param name="fileName">full path to file to has all or part of</param>
        /// <param name="bytesToHash"># of bytes to hash. if 0, hash entire file</param>
        /// <returns></returns>
        private static string GetFileHash(string fileName, long bytesToHash)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if(fileInfo.Length < bytesToHash)
            {
                bytesToHash = fileInfo.Length;
            }

            FileStream fileToHash = File.OpenRead(fileName);

            byte[] chunkToHash = new byte[bytesToHash];

            fileToHash.Read(chunkToHash, 0, (int)bytesToHash);

            fileToHash.Close();

            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(chunkToHash));
        }
        private static string GetFileHash(string fileName)
        {
            return GetFileHash(fileName, 0);
        }
    }

}

