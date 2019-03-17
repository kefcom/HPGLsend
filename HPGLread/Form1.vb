Public Class Form1
    Dim stopButton As Integer
    Dim lineCounter As Integer
    Dim seconds As Integer


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        OFD1.Title = "Choose file"
        OFD1.Filter = "HPGL Files (*.hpgl)|*.hpgl|All Files (*.*)|*.*"
        OFD1.FileName = ""
        OFD1.ShowDialog()
        If OFD1.FileName = "" Then
            MsgBox("no file selected", MsgBoxStyle.Critical, "Stop")
            End
        End If
        Dim FILE_NAME As String = OFD1.FileName
        Dim objReader As New System.IO.StreamReader(FILE_NAME)
        Dim txtcontent As String
        Dim tempstring As String
        Dim commapos As Integer
        Dim coord1 As String
        Dim coord2 As String
        Dim sendstring As String
        Dim puorpd As String
        Dim endofloop As Boolean





        txtcontent = objReader.ReadToEnd

        objReader.Close()

        txtcontent = Replace(txtcontent, ";", "; ", 1, -1, CompareMethod.Text)


        Dim line() As String, i As Integer

        line = Split(txtcontent, " ")

        For i = LBound(line) To UBound(line)
            If (Mid(line(i), 1, 2) = "PU" Or Mid(line(i), 1, 2) = "PD") Then
                endofloop = False
                tempstring = line(i)
                If (Mid(line(i), 1, 2) = "PU") Then puorpd = "PU"
                If (Mid(line(i), 1, 2) = "PD") Then puorpd = "PD"
                tempstring = Replace(tempstring, "PD", "", 1, -1, CompareMethod.Text)
                tempstring = Replace(tempstring, "PU", "", 1, -1, CompareMethod.Text)
                Do
                    'find first occurence of comma
                    commapos = InStr(tempstring, ",", CompareMethod.Text)
                    coord1 = Mid(tempstring, 1, (commapos - 1))
                    tempstring = Strings.Right(tempstring, Len(tempstring) - commapos)
                    commapos = InStr(tempstring, ",", CompareMethod.Text)
                    If (commapos = 0) Then
                        commapos = InStr(tempstring, ";", CompareMethod.Text)
                        If (commapos = 0) Then
                            MsgBox("something went wrong when trying to split coordinates :'(", MsgBoxStyle.Critical, "Error!")
                            End
                        End If
                    End If
                    coord2 = Mid(tempstring, 1, (commapos - 1))
                    sendstring = puorpd & coord1 & "," & coord2 & ";"
                    ListBox1.Items.Add(sendstring)
                    tempstring = Strings.Right(tempstring, Len(tempstring) - commapos)

                    If (Len(tempstring) < 2) Then
                        endofloop = True
                    End If
                Loop Until endofloop = True
            Else
                ListBox1.Items.Add(line(i))
            End If

        Next i
        ProgressBar1.Maximum = ListBox1.Items.Count
        lineCounter = 0
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim found As Integer
        If SerialPort1.IsOpen = False Then
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
            Else
                Button1.Enabled = False
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Button2.Text = "auto send" Then
            stopButton = 0
            Timer1.Interval = txtdelay.Text
            Timer1.Enabled = True
            Button2.Text = "stop!"
        Else
            stopButton = 1
            Timer1.Enabled = False
            Button2.Text = "auto send"
        End If

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If lineCounter < ListBox1.Items.Count Then
            If stopButton = 0 Then
                SerialPort1.Write(ListBox1.Items.Item(lineCounter))
                lineCounter = lineCounter + 1
                ProgressBar1.Value = lineCounter
                Label4.Text = "Processed: " & lineCounter & " of " & ListBox1.Items.Count
                seconds = Int(((ListBox1.Items.Count - lineCounter) * txtdelay.Text) / 1000)
                Label2.Text = "ETA: " & seconds & " Seconds (" & Math.Round((seconds / 60), 0) & " min)"
                Label4.Refresh()
                Label2.Refresh()
                Me.Refresh()
            Else
                Exit Sub
            End If
        Else
            Timer1.Enabled = False
            MsgBox("Sending has been completed.", MsgBoxStyle.Information, "finished!")
            SerialPort1.Close()
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        seconds = Int(((ListBox1.Items.Count - lineCounter) * txtdelay.Text) / 1000)
        MsgBox("at this rate, it will take " & seconds & " Seconds (" & Int(seconds / 60) & " minutes) to process the entire file.")
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Clipboard.SetText(ListBox1.SelectedItem.ToString)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        SerialPort1.Write(ListBox1.Items.Item(lineCounter))
        lineCounter = lineCounter + 1
        ProgressBar1.Value = lineCounter
        Label4.Text = "Processed: " & lineCounter & " of " & ListBox1.Items.Count
        Label4.Refresh()
        Me.Refresh()
        If (lineCounter > 3) Then
            Button2.Enabled = True
        End If
    End Sub

End Class
