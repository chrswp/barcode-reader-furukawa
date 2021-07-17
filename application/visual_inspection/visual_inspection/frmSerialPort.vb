Public Class frmSerialPort

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If cmbBoxPort.Text = "" Then
            MsgBox("Serial Port Name must selected first!", MsgBoxStyle.Critical)
        Else
            frmMain.sSerialPortName = cmbBoxPort.Text
            Me.Close()
        End If
    End Sub

    Private Sub frmSerialPort_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        For Each sp As String In My.Computer.Ports.SerialPortNames
            cmbBoxPort.Items.Add(sp)
        Next
    End Sub

    
End Class