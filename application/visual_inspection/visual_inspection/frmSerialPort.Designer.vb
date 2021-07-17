<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSerialPort
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.cmbBoxPort = New System.Windows.Forms.ComboBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'cmbBoxPort
        '
        Me.cmbBoxPort.FormattingEnabled = True
        Me.cmbBoxPort.Location = New System.Drawing.Point(12, 12)
        Me.cmbBoxPort.Name = "cmbBoxPort"
        Me.cmbBoxPort.Size = New System.Drawing.Size(259, 21)
        Me.cmbBoxPort.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(196, 39)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'frmSerialPort
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 245)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.cmbBoxPort)
        Me.Name = "frmSerialPort"
        Me.Text = "Serial Port Setting"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cmbBoxPort As System.Windows.Forms.ComboBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
