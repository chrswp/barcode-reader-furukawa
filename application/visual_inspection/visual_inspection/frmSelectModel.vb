Imports System.Configuration
Imports System.Reflection

Public Class frmSelectModel

    Private Sub frmSelectModel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'get file model.csv and put to data table dtModel
        ' frmMain.getCSV("model.csv", "getModelName")
        gridViewModel.DataSource = frmMain.dtModel
    End Sub

    Private Sub gridViewModel_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridViewModel.CellContentClick
        Dim i = gridViewModel.CurrentRow.Index
        Dim sText As String = gridViewModel.Item(0, i).Value
        frmMain.strSelectedModel = sText
        frmMain.label_selectedModel.Text = sText


    End Sub

    Private Sub gridViewModel_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles gridViewModel.CellDoubleClick
        Dim i = gridViewModel.CurrentRow.Index
        Dim sText As String = gridViewModel.Item(0, i).Value


        frmMain.strSelectedModel = sText
        frmMain.label_selectedModel.Text = sText
        frmMain.iIndex = gridViewModel.CurrentRow.Index
        updateConfig(sText)
        Me.Close()
    End Sub

    Private Sub updateConfig(ByVal sText As String)
        Dim _appConfig As New AppConfig

        _appConfig.SetProperty("RUNNING MODEL", sText)

    End Sub
End Class