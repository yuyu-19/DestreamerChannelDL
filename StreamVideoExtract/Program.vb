Imports System
Imports System.IO

Module Program
    Dim ParentDir As String = Environment.CurrentDirectory
    Sub Main(args As String())
        Dim args2 As List(Of String) = args.ToList
        Dim threads As Integer = 1
        Dim di As DirectoryInfo
        If File.Exists(ParentDir & "\isdev.txt") Then    'Autopopulates it for my specific setup because I'm lazy AF lmao
            di = New DirectoryInfo(ParentDir)
            For Each f As FileInfo In di.GetFiles
                If f.Name.Contains("data-") Then
                    args2.Insert(0, f.FullName)
                    args2.Insert(1, """" & Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\Programs\Destreamer\destreamer\destreamer.cmd""")
                    args2.Insert(2, """I:\Lezioni\" & f.Name.Replace("data-", "").Replace(".txt", "") & """")
                End If
            Next

        End If

        If args2.Count < 3 Then
            Console.WriteLine("Usage: streamchannelDL.exe ""path-to-file"" ""path-to-destreamer.cmd"" ""output-dir"" [--threads X]" & vbCrLf)
            Console.WriteLine("The stream site is complicated and I'm too dumb to figure out how to load the entire" & vbCrLf & "list of videos, so you'll have to do it yourself. Just scroll to the bottom, do inspect, copy the body and save it to a file.")
            Console.WriteLine("You can also optionally add --threads X to parallelize it")
            Return
        End If

        Dim i As Integer

        For i = 0 To args.Length - 1
            If args(i) = "--threads" Then
                If args.Length > i + 1 Then
                    threads = args(i + 1)
                End If
            End If
        Next


        If args2(0) = "" Or args2(1) = "" Or args2(2) = "" Then
            Console.WriteLine("Usage: streamchannelDL.exe ""path-to-file"" ""path-to-destreamer.cmd"" ""output-dir""")
            Console.WriteLine("The stream site is weird and I'm too dumb to figure out how to load the entire list of videos,_
             so you'll have to do it yourself. Just scroll to the bottom, do inspect, copy the body and save it to a file.")
        End If

        For i = 0 To args2.Count - 1
            args2(i) = args2(i).Replace("""", "")
        Next


        Dim text As String = ""

        Try
            text = File.ReadAllText(args2(0))  'Why yes this is a hackjob done in five minutes, why do you ask?
        Catch ex As Exception
            Console.WriteLine("Error reading file.")
            Console.ReadLine()
            Console.WriteLine(ex.ToString)
            Console.ReadLine()
        End Try

        Dim URLs As New List(Of String)
        i = text.IndexOf(" href=""/video/")
        Dim i2 As Integer

        Do Until i = -1
            i = text.IndexOf("""", i)
            i2 = text.IndexOf("?", i + 1)
            Console.WriteLine(text.Substring(i + 1, i2 - i - 1))
            If URLs.Count > 0 Then
                If URLs(URLs.Count - 1) <> ("https://web.microsoftstream.com" & text.Substring(i + 1, i2 - i - 1)) Then 'Avoid repeat elements
                    URLs.Add("https://web.microsoftstream.com" & text.Substring(i + 1, i2 - i - 1))
                End If
            Else
                URLs.Add("https://web.microsoftstream.com" & text.Substring(i + 1, i2 - i - 1))
            End If

            i = text.IndexOf(" href=""/video/", i)
        Loop

        If Not (Directory.Exists(ParentDir & "\DirURLs")) Then
            Directory.CreateDirectory(ParentDir & "\DirURLs")
        End If

        di = New DirectoryInfo(ParentDir & "\DirURLs")
        For Each f As FileInfo In di.GetFiles
            f.Delete()
        Next

        For Each s As String In URLs
            File.AppendAllText(ParentDir & "\DirURLs\urls" & (URLs.IndexOf(s) Mod threads) & ".txt", s & vbCrLf)
        Next
        For i = 0 To threads
            If File.Exists(ParentDir & "\DirURLs\urls" & i & ".txt") Then
                RunCommandH(args2(1), " -f """ & ParentDir & "\DirURLs\urls" & i & ".txt"" -k -o """ & args2(2) & """ -x -v --format mp4 --skip")
            End If
        Next

    End Sub


    Function RunCommandH(Command As String, Arguments As String) As String
        Console.WriteLine(Command & Arguments)
        'Console.ReadLine()
        Dim oProcess As New Process()
        Dim oStartInfo As New ProcessStartInfo(Command, Arguments)
        oStartInfo.WorkingDirectory = Command.Substring(0, Command.LastIndexOf("\"))
        oStartInfo.UseShellExecute = True
        oStartInfo.RedirectStandardOutput = False
        oProcess.StartInfo = oStartInfo

        Try
            oProcess.Start()
        Catch ex As Exception
            File.WriteAllText(ParentDir & "crashreport.txt", ex.ToString)
            Console.WriteLine(ParentDir)
            Console.WriteLine(ex.ToString)
            Console.WriteLine("Something went wrong.")
            Return "Error"
            Console.ReadLine()
        End Try



        'oProcess.WaitForExit()

        'oProcess.Dispose()

        Return ""
    End Function
End Module
