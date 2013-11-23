* About this program

This program downloads PDF from jw.org, and adds "Table of contents".



* Usage

Run the program and select language and publication.

Push "Download PDF" to download a PDF from jw.org.

Push "Convert PDF" to create a PDF that have TOC from downloaded PDF.

Double click a publication to open output folder.



* Language and publication

publication.xml has language and publication list.
Text files in toc folder have TOC data of each publications.

To add language or publication, you can modify publication.xml and create TOC file.



* Requirements and Notes

.NET framework 2.0 or .NET framework 3.0 or .NET framework 3.5

** jpdfbookmarks
This program uses a command line tool of 'jpdfbookmarks' to add TOC.
Java Runtime Environment is required to run jpdfbookmarks.

jpdfbookmarks is downloaded by this program automatically.
http://sourceforge.net/projects/jpdfbookmarks/files/JPdfBookmarks-2.5.2/

jpdfbookmarks is called like followings:
jpdfbookmarks\jpdfbookmarks_cli.exe jw_org\bh_E.pdf -e UTF-8 -a toc\bh_E.txt -o output\bh_E.pdf

** Ionic.Zip.dll
Ionic.Zip.dll is included in archive.
It is used to extract a zip archive of jpdfbookmarks.



* License

c.f. LISENCE.txt
