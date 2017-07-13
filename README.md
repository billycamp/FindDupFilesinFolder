# FindDupFilesinFolder


This is an implementation of an answer to the following question:

A folder contains a large # (~50k) of files.  Write a C# program that finds files with duplicate contents.

My answer was to do a first pass of all the files, take a md5 hash of a small chunk (4k bytes) of each file.  Store 1st instance of hash & filename, all others with matching hash get their filename & the potential match filename added to a list of potential duplicates.
On the second pass, do full MD5 hash of each potential duplicate file & compare.  Output resulting match list.

 
