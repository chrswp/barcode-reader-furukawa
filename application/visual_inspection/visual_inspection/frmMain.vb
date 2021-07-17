Imports System
Imports System.IO.File
Imports System.IO.StreamReader
Imports System.IO
Imports System.IO.Ports ' for system with serial ports
Imports System.Threading
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Diagnostics
Imports System.Timers.Timer
Imports System.Net
Imports System.Net.Sockets
Imports System.Net.Mail
Imports System.Text
'Imports System.Data.SQLite
Imports System.Net.NetworkInformation
Imports System.Reflection
Imports System.DateTime
Public Class frmMain
    Public sAppLog As String = "AppVisualCheck.log"
    Public dtTemp As New DataTable 'Temporary
    Public dtModel As New DataTable
    Public iIndex As Integer = 0
    Declare Function AttachConsole Lib "kernel32.dll" (ByVal dwProcessId As Int32) As Boolean 'attach console output to dos prompt windows
    Declare Function FreeConsole Lib "kernel32.dll" () As Boolean 'release console
    Private Declare Function GetTickCount Lib "kernel32" () As Long
    Public strSelectedModel As String = ""
    Public bStatus As Boolean = False
    Public sSerialPortName As String = "COM1" 'default serial port name
    Public COM1 As IO.Ports.SerialPort
    Dim settingReader As New AppSettingsReader 'APP CONFIG READER
    Public fileWatcher As FileSystemWatcher
    Dim sKeyenceFolder As String = "D:\KEYENCE IMAGE FILE\" 'folder file location
    Dim sFurukawaFolder As String = "D:\FURUKAWA IMAGE FILE\"
    Dim sFileName As String = "" 'file name created by Keyence Camera which need to rename 
    Dim sDate As String = Now.Date.ToString("dd-MM-yyyy")
    Dim _appConfig As New AppConfig

    Private Sub txtBarcode_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBarcode.TextChanged
        If InStr(txtBarcode.Text, vbLf) > 0 Then
            txtBarcode.Text = txtBarcode.Text.Substring(0, (Len(txtBarcode.Text) - 1))
            txtBarcode.SelectAll() 'select all text, sehingga nanti saat ada data barcode baru langsung over write text yang lama
            label_barcode.Text = txtBarcode.Text.Substring(0, (Len(txtBarcode.Text) - 1))
            bStatus = checkBarcode(label_barcode.Text)
            ' If bStatus = True Then StartCamera()
            Thread.Sleep(300)
        End If
    End Sub
    Public Sub writeAppLog(ByVal sString As String)
        If File.Exists(sAppLog) Then
            Using writer As New StreamWriter(sAppLog, True)
                writer.WriteLine(DateTime.Now & " :: " & sString)
            End Using
        Else
            Using fileSys As IO.FileStream = File.Create(sAppLog)

            End Using
            Using writer As New StreamWriter(sAppLog, True)
                writer.WriteLine(DateTime.Now & " :: " & sString)
            End Using
        End If
    End Sub
    Private Sub frmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        
        'GET PARAMETER FROM APP CONFIG

        strSelectedModel = _appConfig.GetProperty("RUNNING MODEL")
        sFurukawaFolder = _appConfig.GetProperty("FURUKAWA IMAGE FOLDER")
        sKeyenceFolder = _appConfig.GetProperty("KEYENCE IMAGE FOLDER")
        label_selectedModel.Text = strSelectedModel

        'Prepare folder for data storage
        createFolder()

        'GET CSV FILE
        getCSV("model.csv", "getModelName")
        'check serial port name, if only get 1 port then set it to the serial port name
        Dim i As Integer = 0 'counter

        'CHECK AND OPEN SERIAL PORT
        checkSerialPort()

        'START FILE WATCHER SYSTEM
        'start watchfolder services 
        'untuk mendeteksi apakah ada perubahan pada file order menu
        fileWatcher = New System.IO.FileSystemWatcher
        fileWatcher.Path = sKeyenceFolder
        fileWatcher.NotifyFilter = IO.NotifyFilters.DirectoryName
        fileWatcher.NotifyFilter = fileWatcher.NotifyFilter Or _
                                   IO.NotifyFilters.FileName
        fileWatcher.NotifyFilter = fileWatcher.NotifyFilter Or _
                                   IO.NotifyFilters.Attributes
        AddHandler fileWatcher.Changed, AddressOf fileChanged
        AddHandler fileWatcher.Created, AddressOf fileCreated
        AddHandler fileWatcher.Deleted, AddressOf fileDeleted
        fileWatcher.EnableRaisingEvents = True
        fileWatcher.IncludeSubdirectories = True

        
    End Sub
    Private Sub fileChanged()
        ToolStripStatusLabel1.Text = "file changed"
    End Sub
    Private Sub fileCreated(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs)
        ToolStripStatusLabel1.Text = "file created"
        'Get File Name just created by Keyence camera, copy to another folder and rename it follow Furukawa's standard
        sFileName = e.FullPath().ToString
        Thread.Sleep(300)
        Dim sTargetFileName = sFurukawaFolder & label_selectedModel.Text & "_" & Date.Now.ToString("HHmmss") & ".img"
        My.Computer.FileSystem.CopyFile(sFileName, sTargetFileName, overwrite:=True)

    End Sub
    Private Sub fileDeleted()
        ToolStripStatusLabel1.Text = "file deleted"
    End Sub
    Public Sub getCSV(ByVal sFileCSV As String, ByVal sPurpose As String) 'sPurpose is to identify purpose of CSV FILE--> CSV KARYAWAN OR ORDER
        'getting CSV file & add to datatable
        Dim i, j As Integer
        Dim nr As DataRow
        Dim colCount As Integer

        'flushing data from myTable --> make empty buffer data
        dtTemp = New DataTable 'Clear temporary data table ---> INI SEBENARNYA UNTUK MEMBUAT DATA TABLE YANG DIBUAT SELALU KOSONG, Karena DTTEMP ini adalah EMPTY
        If Not File.Exists(sFileCSV) Then
            MsgBox("Error when try to open file " & sFileCSV & ". There is no existing file. Please check its file.", MsgBoxStyle.Exclamation)
        End If
        Console.WriteLine("try to reading csv file")

        Try
            Using MyReader As New Microsoft.VisualBasic.
                            FileIO.TextFieldParser(
                              sFileCSV)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim currentRow As String()
                i = 0 'row counter
                j = 0
                While Not MyReader.EndOfData
                    currentRow = MyReader.ReadFields()
                    colCount = currentRow.Length
                    If colCount <= 0 Then Exit Sub ' BECAUSE IT DO NOT HAVE ANY FIELD!!
                    '///ADD COLUMN TO TEMPORARY DATATABLE\\\\
                    If dtTemp.Columns.Count < colCount Then
                        For i = 0 To colCount - 1
                            dtTemp.Columns.Add()
                        Next
                    Else
                        'nothing to do
                    End If
                    Dim currentField As String
                    nr = dtTemp.NewRow
                    For Each currentField In currentRow
                        nr(j) = currentField
                        'MsgBox(currentField)
                        ' Console.WriteLine("Read data csv " & sPurpose & ": " & currentField)
                        If j < colCount - 1 Then
                            j += 1
                        Else
                            j = 0 'reset field counter
                            dtTemp.Rows.Add(nr)
                            'DEBUG PURPOSE ONLY
                            '  Console.WriteLine("add data to table from file " & sPurpose)
                        End If
                    Next
                    i += 1
                End While
            End Using


            Select Case sPurpose
                Case "getModelName"
                    dtModel = dtTemp 'COPY CONTENT OF TEMPORARY DATA TABLE TO DATA TABLE KARYAWAN (DTMAIN)
                    dtModel.Columns(0).ColumnName = "MODEL"
                    dtModel.Columns(1).ColumnName = "BARCODE"
                    dtModel.Columns(2).ColumnName = "FILE IMAGE LOCATION" 'prepare for RFID 
                    dtModel.AcceptChanges()
            End Select
            Console.WriteLine("file csv loaded successfully.")
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & "FATAL ERROR while reading csv file @sub getCSV")
            Console.WriteLine("FATAL ERROR while reading csv file @sub getCSV --> " & vbCrLf & Err.Number & Space(2) & ex.Message)
            writeAppLog("FATAL ERROR csv file @sub getCSV --> " & vbCrLf & Err.Number & Space(2) & ex.Message)
        End Try
    End Sub
    Private Sub ToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem1.Click
        frmSelectModel.Show()
    End Sub
    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Function checkBarcode(ByVal sBarcode As String)
        Dim bCheck As Boolean
        Dim ndx As Int32 'row index for searching data table
        Dim expression As String = "BARCODE = '" + sBarcode + "'"
        Dim ar_rows() As DataRow = dtModel.Select(expression)
        Dim sModelName As String

        If label_selectedModel.Text = "RUNNING MODEL NAME" Then
            pictureBox1.Image = WindowsApplication1.My.Resources.Resources.bad
            MsgBox("Please select running model name first", MsgBoxStyle.Critical)

            Return False
            Exit Function 'exit function due to model name not selected by user
        End If

        If ar_rows.Length > 0 Then
            ndx = dtModel.Rows.IndexOf(ar_rows(0))
        End If
        If ndx <> 0 Then
            sModelName = dtModel.Rows(ndx)(0)  ' kolom nama
            'compare model name found from barcode with running model name
            If sModelName = label_selectedModel.Text Then
                bCheck = True
                pictureBox1.Image = WindowsApplication1.My.Resources.Resources.ok_
                StartCamera()
            Else
                bCheck = False
                pictureBox1.Image = WindowsApplication1.My.Resources.Resources.bad
            End If
        End If
        Return bCheck
    End Function

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        Application.Restart()
    End Sub

    Private Sub checkSerialPort()
        Dim i As Integer
        For Each sp As String In My.Computer.Ports.SerialPortNames
            i += 1
            sSerialPortName = sp
        Next
        If i < 1 Then
            MsgBox("Found no serial port! Application stopped.", MsgBoxStyle.Critical)
            Application.Exit()
        End If

        Try
            COM1 = My.Computer.Ports.OpenSerialPort(sSerialPortName)
            COM1.ReadTimeout = 10000
            ToolStripStatusLabel1.Text = "Serial Port " & sSerialPortName & " : OPEN"
        Catch ex As TimeoutException
            MsgBox("Error: Serial Port read timed out.", MsgBoxStyle.Critical)
        Finally
            If COM1 IsNot Nothing Then COM1.Close()
        End Try
    End Sub

    Private Sub SettingSerialPortToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SettingSerialPortToolStripMenuItem.Click
        frmSerialPort.Show()
    End Sub

    Private Sub StartCamera()
        If COM1.IsOpen Then
            COM1.Write("1")
            Thread.Sleep(200)
            COM1.Write("0")
            ToolStripStatusLabel1.Text = "Switching through Serial Port " & sSerialPortName & " is OK"
        ElseIf Not COM1.IsOpen Then
            Try
                COM1.Open()
                ToolStripStatusLabel1.Text = "Switching through Serial Port " & sSerialPortName & " is OK"
                COM1.Write("1")
                Thread.Sleep(200)
                COM1.Write("0")
            Catch ex As Exception
                MsgBox("Fail to switch on camera through serial port")
            End Try
        End If
    End Sub

    Private Sub Delay(ByVal Length As Long)
        Dim OldTime As Long
        OldTime = GetTickCount
        Do
            Application.DoEvents()
            If GetTickCount >= OldTime + Length Then Exit Do
        Loop
    End Sub

    Private Sub createFolder()

        'create folder for daily based storage. Folder name follow current date

        'in case these folder doesn't exist, then create it
        If Not Directory.Exists(sKeyenceFolder) Then
            'CREATE FOLDER TRANSAKSI AND ITS SUB FOLDER  and file transaction altogether
            Directory.CreateDirectory(sKeyenceFolder) 'MOTHER DIRECTORY TRANSAKSI
        End If

        If Not Directory.Exists(sFurukawaFolder) Then
            'CREATE FOLDER TRANSAKSI AND ITS SUB FOLDER  and file transaction altogether
            Directory.CreateDirectory(sFurukawaFolder) 'MOTHER DIRECTORY TRANSAKSI
            Directory.CreateDirectory(Date.Now.ToString(sDate))

        ElseIf Directory.Exists(sFurukawaFolder) Then
            If Not Directory.Exists(sDate) Then
                'CREATE FOLDER PAGI & FILE TRANSAKSI PER TODAY
                Directory.CreateDirectory(sDate) 'create folder then create file transaction record for today
            End If
        End If
    End Sub

End Class