Public Class Form1
    Dim stopknop As Integer
    Dim teller As Integer
    Dim seconds As Integer


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        OFD1.Filter = "HPGL Files (*.hpgl)|*.hpgl|All Files (*.*)|*.*"
        OFD1.ShowDialog()
        Dim FILE_NAME As String = OFD1.FileName
        Dim objReader As New System.IO.StreamReader(FILE_NAME)
        Dim txtinhoud As String


        txtinhoud = objReader.ReadToEnd

        objReader.Close()

        txtinhoud = Replace(txtinhoud, ";", "; ", 1, -1, CompareMethod.Text)


        Dim line() As String, i As Integer

        line = Split(txtinhoud, " ")

        For i = LBound(line) To UBound(line)
            ListBox1.Items.Add(line(i))
        Next i
        ProgressBar1.Maximum = ListBox1.Items.Count

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim found As Integer

        SerialPort1.PortName = "COM" & compoort.Text
        SerialPort1.Close()
        found = 0
        Do
            Try
                SerialPort1.Open()
                found = 1
            Catch ex As Exception
                compoort.Text = compoort.Text + 1
            End Try
        Loop Until found = 1 Or compoort.Text > 30
        If found = 0 Then
            MsgBox("Comport not found :'( are you sure it's connected?", MsgBoxStyle.Critical, "Fatal error")
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Button2.Text = "send" Then
            teller = 0
            stopknop = 0
            Timer1.Interval = txtdelay.Text
            Timer1.Enabled = True
            Button2.Text = "stop!"
        Else
            stopknop = 1
            Timer1.Enabled = False
            Button2.Text = "send"
        End If

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If teller < ListBox1.Items.Count Then
            If stopknop = 0 Then
                SerialPort1.Write(ListBox1.Items.Item(teller))
                teller = teller + 1
                ProgressBar1.Value = teller
                Label4.Text = "Processed: " & teller & " of " & ListBox1.Items.Count
                seconds = Int(((ListBox1.Items.Count - teller) * txtdelay.Text) / 1000)
                Label2.Text = "ETA: " & seconds & " Secondsc"
                Label4.Refresh()
                Label2.Refresh()
                Me.Refresh()
            Else
                Exit Sub
            End If
        Else
            MsgBox("done")
            SerialPort1.Close()
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        seconds = Int(((ListBox1.Items.Count - teller) * txtdelay.Text) / 1000)
        MsgBox("at this rate, it will take " & seconds & " Seconds (" & Int(seconds / 60) & " minutes) to process the entire file.")
    End Sub
End Class
