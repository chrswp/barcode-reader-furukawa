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
Imports System.Net.NetworkInformation
Imports System.Reflection

Module getFileCSV
    Public Function readCSV(ByVal sFileCSV As String) As DataTable
        'getting CSV file & add to datatable
        Dim i, j As Integer
        Dim nr As DataRow
        Dim colCount As Integer
        Dim dtTEMP As New DataTable

        'flushing data from myTable --> make empty buffer data
        dtTEMP = New DataTable 'Clear temporary data table ---> INI SEBENARNYA UNTUK MEMBUAT DATA TABLE YANG DIBUAT SELALU KOSONG, Karena DTTEMP ini adalah EMPTY
        If Not File.Exists(sFileCSV) Then
            MsgBox("Error when try to open file " & sFileCSV & ". There is no existing file. Please check its file.", MsgBoxStyle.Exclamation)
        End If
        Console.WriteLine("try to reading csv file")

        Try
            Using MyReader As New Microsoft.VisualBasic.FileIO.TextFieldParser(sFileCSV)
                MyReader.TextFieldType = FileIO.FieldType.Delimited
                MyReader.SetDelimiters(",")
                Dim currentRow As String()
                i = 0 'row counter
                j = 0
                While Not MyReader.EndOfData
                    currentRow = MyReader.ReadFields()
                    colCount = currentRow.Length
                    If colCount <= 0 Then
                        MsgBox("Error when opening file " & sFileCSV & "... column count < 0")
                    End If

                    '///ADD COLUMN TO TEMPORARY DATATABLE\\\\
                    If dtTEMP.Columns.Count < colCount Then
                        For i = 0 To colCount - 1
                            dtTEMP.Columns.Add()
                        Next
                    Else
                        'nothing to do
                    End If
                    Dim currentField As String
                    nr = dtTEMP.NewRow
                    For Each currentField In currentRow
                        nr(j) = currentField
                        'MsgBox(currentField)
                        ' Console.WriteLine("Read data csv " & sPurpose & ": " & currentField)
                        If j < colCount - 1 Then
                            j += 1
                        Else
                            j = 0 'reset field counter
                            dtTEMP.Rows.Add(nr)
                            '  Console.WriteLine("add data to table from file " & sPurpose) 'DEBUG PURPOSE ONLY
                        End If
                    Next
                    i += 1
                End While
            End Using
            Console.WriteLine("file csv loaded successfully.")
        Catch ex As Exception
            MsgBox(ex.Message.ToString & vbCrLf & "ERROR while reading csv file module getFileCSV at sub readCSV")
            Console.WriteLine("FATAL ERROR while reading csv file @sub getCSV --> " & vbCrLf & Err.Number & Space(2) & ex.Message)
            frmMain.writeAppLog("FATAL ERROR csv file @sub getCSV --> " & vbCrLf & Err.Number & Space(2) & ex.Message)
        End Try
        Return dtTEMP
    End Function
End Module
